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

        // cached players, refreshed a few times a second instead of every menu open
        private List<CCSPlayerController> _cachedPlayers = [];
        private float _lastPlayerUpdate;
        private const float PlayerCacheInterval = 0.25f;

        private ILogger _logger = null!;

        public void OnConfigParsed(AOGTrollConfig config)
        {
            _registry.EnsureConfigDefaults(config);
            Config = config;
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

            RegisterEffects();
            _registry.EnsureConfigDefaults(Config);

            if (!Config.EnableTrolling)
                _logger.Warning("EnableTrolling is set to false in config - css_atroll will do nothing until it's re-enabled.");

            _logger.Information("Plugin loaded. Version: {Version}, {Count} effects registered.", ModuleVersion, _registry.All.Count);
            Console.WriteLine("[TROLL] Plugin loaded!");
        }

        public override void Unload(bool hotReload)
        {
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

        /// <summary>
        /// Every effect the plugin ships with gets registered here. To add a new
        /// one: implement ITrollEffect in Effects/BuiltIn, register it below, and
        /// add its lang keys to lang/en.json + lang/ru.json.
        /// </summary>
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

            // Config.RequiresFlag lets server owners change the required flag
            // without recompiling; the attribute above is just the hard floor.
            if (!AdminManager.PlayerHasPermissions(admin, Config.RequiresFlag))
            {
                admin.PrintToChat(Localizer["troll.no_permission"]);
                return;
            }

            OpenPlayerMenu(admin);
        }

        private void Execute(TrollEffectVariant variant, CCSPlayerController admin, CCSPlayerController target, string prefix)
        {
            if (!target.IsValid)
                return;

            TrollEffectContext ctx = new(this, Localizer, _state, admin, target, prefix);
            variant.Run(ctx);
        }
    }
}
