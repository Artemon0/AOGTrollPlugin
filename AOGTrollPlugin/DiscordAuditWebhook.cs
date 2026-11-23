using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Web;
using Serilog;

namespace AOGTrollPlugin;

internal sealed class DiscordAuditWebhook(ILogger logger)
{
    private static readonly Regex WebhookPath = new(
        @"\A/api/(?:v[0-9]+/)?webhooks/[0-9]+/[a-zA-Z0-9_-]+\z",
        RegexOptions.CultureInvariant);

    private readonly Channel<(Uri[] Destinations, string Message)> _queue =
        Channel.CreateBounded<(Uri[], string)>(new BoundedChannelOptions(128) { SingleReader = true });

    private Uri[] _destinations = [];
    private CancellationTokenSource? _shutdown;
    private Task? _worker;

    public void Configure(string[]? urls)
    {
        List<Uri> destinations = [];
        foreach (string? url in urls ?? [])
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) &&
                uri.Scheme == Uri.UriSchemeHttps && uri.Host == "discord.com" && uri.IsDefaultPort &&
                uri.UserInfo.Length == 0 && uri.Fragment.Length == 0 && WebhookPath.IsMatch(uri.AbsolutePath))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                query["wait"] = "true";
                destinations.Add(new UriBuilder(uri) { Query = query.ToString() }.Uri);
                continue;
            }

            logger.Warning("Ignoring an invalid Discord audit webhook URL.");
        }

        _destinations = destinations.Distinct().ToArray();
        if (_destinations.Length > 0 && _worker == null)
        {
            _shutdown = new CancellationTokenSource();
            CancellationToken token = _shutdown.Token;
            _worker = Task.Run(() => SendQueuedAsync(token));
        }
    }

    public void Enqueue(string message)
    {
        if (_destinations.Length > 0 && !_queue.Writer.TryWrite((_destinations, message)))
            logger.Warning("Discord audit queue is full. The action is still in the local log.");
    }

    public void Stop()
    {
        _queue.Writer.TryComplete();
        if (_shutdown is { } shutdown && _worker is { } worker)
        {
            shutdown.Cancel();
            _ = DisposeAfterStopAsync(worker, shutdown);
        }
    }

    private static async Task DisposeAfterStopAsync(Task worker, CancellationTokenSource shutdown)
    {
        using (shutdown)
            await worker.ConfigureAwait(false);
    }

    private async Task SendQueuedAsync(CancellationToken cancellationToken)
    {
        using HttpClient client = new(new HttpClientHandler { AllowAutoRedirect = false })
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        try
        {
            await foreach (var (destinations, message) in _queue.Reader.ReadAllAsync(cancellationToken))
            {
                var payload = new
                {
                    content = $"```\n{message.Replace('`', 'ˋ')}\n```",
                    allowed_mentions = new { parse = Array.Empty<string>() }
                };

                foreach (Uri destination in destinations)
                {
                    try
                    {
                        using HttpResponseMessage response = await client.PostAsJsonAsync(destination, payload, cancellationToken);
                        if (!response.IsSuccessStatusCode)
                            logger.Warning("Discord audit delivery returned HTTP {StatusCode}.", (int)response.StatusCode);

                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            TimeSpan delay = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(2);
                            if (delay > TimeSpan.Zero)
                                await Task.Delay(delay, cancellationToken);
                        }
                    }
                    catch (HttpRequestException)
                    {
                        logger.Warning("Discord audit delivery failed due to an HTTP connection error.");
                    }
                    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                        logger.Warning("Discord audit delivery timed out.");
                    }
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}
