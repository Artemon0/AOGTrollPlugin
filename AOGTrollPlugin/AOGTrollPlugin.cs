using System;
using System.Collections.Generic;
using System.Linq;
using AOGTrollPlugin.Effects;
using AOGTrollPlugin.Effects.BuiltIn;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using MenuManager;
using Serilog;

namespace AOGTrollPlugin
{
    public partial class AOGTrollPlugin : BasePlugin, IPluginConfig<AOGTrollConfig>
    {
        public override string ModuleName => "AOG Troll Plugin";
        public override string ModuleVersion => typeof(AOGTrollPlugin).Assembly.GetName().Version?.ToString(3) ?? "unknown";
        public override string ModuleAuthor => "AOGames888";

        public AOGTrollConfig Config { get; set; } = new();

        private readonly EffectRegistry _registry = new();
        private readonly TrollStateStore _state = new();
        private ConnectionProblemsEffect _connectionProblems = null!;

        private readonly PluginCapability<IMenuApi?> _menuApiCapability = new("menu:nfcore");
        private IMenuApi? _menuApi;

        private List<CCSPlayerController> _cachedPlayers = [];
        private float _lastPlayerUpdate;
        private const float PlayerCacheInterval = 0.25f;

        private ILogger _logger = null!;
        private DiscordAuditWebhook? _discordAudit;

        public void OnConfigParsed(AOGTrollConfig config)
        {
            _registry.EnsureConfigDefaults(config);
            Config = config;
            _discordAudit?.Configure(config.DiscordWebhookUrls);
        }

        public override void Load(bool hotReload)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    "logs/AOGTrollPlugin.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            _discordAudit = new DiscordAuditWebhook(_logger);
            _discordAudit.Configure(Config.DiscordWebhookUrls);

            RegisterEffects();
            _registry.EnsureConfigDefaults(Config);

            if (!Config.EnableTrolling)
                _logger.Warning("EnableTrolling is set to false in config - css_atroll will do nothing until it's re-enabled.");

            _logger.Information("Plugin loaded. Version: {Version}, {Count} effects registered.", ModuleVersion, _registry.All.Count);
            Console.WriteLine("[TROLL] Plugin loaded!");
        }

        public override void Unload(bool hotReload)
        {
            _discordAudit?.Stop();
            _discordAudit = null;
            _connectionProblems.StopAll();
            _state.ClearAll();
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            _menuApi = _menuApiCapability.Get();

            if (_menuApi == null)
            {
                _logger.Warning("MenuManagerCS2 capability unavailable; using CenterHtmlMenu. Install MenuManagerCore and its shared API.");
            }
        }

        private List<CCSPlayerController> GetCachedPlayers()
        {
            if (!(Server.CurrentTime - _lastPlayerUpdate > PlayerCacheInterval))
                return _cachedPlayers;

            _cachedPlayers = Utilities.GetPlayers()
                .Where(p => p is { IsValid: true, IsBot: false } && p.Team != CsTeam.Spectator)
                .ToList();
            _lastPlayerUpdate = Server.CurrentTime;
            return _cachedPlayers;
        }

        [ConsoleCommand("css_atroll", "Troll a player")]
        [RequiresPermissions("@css/troll")]
        public void TrollPlayer(CCSPlayerController? admin, CommandInfo command)
        {
            if (admin == null || !admin.IsValid)
                return;

            if (!Config.EnableTrolling)
                return;

            if (!AdminManager.PlayerHasPermissions(admin, Config.RequiresFlag))
            {
                admin.PrintToChat(Localizer["troll.no_permission"]);
                return;
            }

            OpenPlayerMenu(admin);
        }

        private void Execute(ITrollEffect effect, TrollEffectVariant variant, CCSPlayerController admin, CCSPlayerController target, string prefix)
        {
            if (!admin.IsValid || !target.IsValid || !Config.EnableTrolling || !EffectRegistry.IsEnabled(effect, Config))
                return;

            if (!AdminManager.PlayerHasPermissions(admin, "@css/troll") || !AdminManager.PlayerHasPermissions(admin, Config.RequiresFlag))
            {
                admin.PrintToChat(Localizer["troll.no_permission"]);
                return;
            }

            TrollEffectContext ctx = new(this, Localizer, _state, admin, target, prefix);
            if (effect is RandomEffect)
            {
                variant.Run(ctx);
                return;
            }

            RunEffect(effect, variant, ctx, "menu");
        }

        private void RunRandomEffect(TrollEffectContext ctx)
        {
            var candidates = _registry.Enabled(Config)
                .Where(effect => effect.IncludeInRandomSelection && effect is not RandomEffect &&
                    effect.Category is not (EffectCategory.AntiCheat or EffectCategory.Streamer))
                .Select(effect => (Effect: effect, Variants: effect.GetVariants()))
                .Where(candidate => candidate.Variants.Count > 0)
                .ToArray();

            if (candidates.Length == 0)
            {
                ctx.PrintToAdmin("troll.random.empty");
                return;
            }

            var (effect, variants) = candidates[Random.Shared.Next(candidates.Length)];
            TrollEffectVariant variant = variants[Random.Shared.Next(variants.Count)];
            string label = ctx.Localizer[variant.LabelLocalizationKey];
            ctx.Admin.PrintToChat($" {ctx.ChatPrefix} {ctx.Localizer["troll.random.selected", label]}");
            RunEffect(effect, variant, ctx, "random");
        }

        private void RunEffect(ITrollEffect effect, TrollEffectVariant variant, TrollEffectContext ctx, string source)
        {
            string adminName = SanitizeAuditName(ctx.Admin.PlayerName);
            string targetName = SanitizeAuditName(ctx.Target.PlayerName);
            ulong adminId = ctx.Admin.SteamID;
            ulong targetId = ctx.Target.SteamID;
            string map = Server.MapName;

            _logger.Information(
                "Admin {AdminName} ({AdminSteamId}) invoked effect {EffectId} ({Variant}) on {TargetName} ({TargetSteamId}) via {Source} on {Map}",
                adminName, adminId, effect.Id, variant.LabelLocalizationKey, targetName, targetId, source, map);

            _discordAudit?.Enqueue(
                $"{DateTimeOffset.UtcNow:u} | {map}\nAdmin: {adminName} ({adminId})\nTarget: {targetName} ({targetId})\n" +
                $"Invoked: {effect.Id} ({variant.LabelLocalizationKey})\nSource: {source}");

            variant.Run(ctx);
        }

        private static string SanitizeAuditName(string name) =>
            new(name.Select(character => char.IsControl(character) ? ' ' : character).ToArray());
    }
}
