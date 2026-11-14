using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;

namespace AOGTrollPlugin.Effects
{
    /// <summary>
    /// Everything an effect needs to run, bundled up so effect classes don't
    /// need to depend on the main plugin class directly.
    /// </summary>
    public sealed class TrollEffectContext(
        BasePlugin plugin,
        IStringLocalizer localizer,
        TrollStateStore state,
        CCSPlayerController admin,
        CCSPlayerController target,
        string chatPrefix)
    {
        public BasePlugin Plugin { get; } = plugin;
        public IStringLocalizer Localizer { get; } = localizer;
        public TrollStateStore State { get; } = state;
        public CCSPlayerController Admin { get; } = admin;
        public CCSPlayerController Target { get; } = target;
        public string ChatPrefix { get; } = chatPrefix;

        public void PrintToTarget(string localizationKey) =>
            Target.PrintToChat($" {ChatPrefix} {Localizer[localizationKey]}");

        public void PrintToAdmin(string localizationKey) =>
            Admin.PrintToChat($" {ChatPrefix} {Localizer[localizationKey]}");
    }
}
