using System.Collections.Generic;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin
{
    public class AOGTrollConfig : BasePluginConfig
    {
        [JsonPropertyName("EnableTrolling")]
        public bool EnableTrolling { get; set; } = true;

        [JsonPropertyName("RequiresFlag")]
        public string RequiresFlag { get; set; } = "@css/troll";

        /// <summary>
        /// Sound event name played by AnnoyingSoundEffect (troll.annoying_sound).
        /// CS2 plays sounds by soundevent name (from the game's .vsndevts files),
        /// not by a raw file path - the default below is a stock CS2 UI sound
        /// that should exist on any server, but if it doesn't play, open your
        /// game's soundevents in Source 2 Viewer and swap in any event name you
        /// find there.
        /// </summary>
        [JsonPropertyName("SoundEffectEvent")]
        public string SoundEffectEvent { get; set; } = "UI.WeaponPurchase";

        /// <summary>
        /// Per-effect on/off switches, keyed by effect id (e.g. "skybox", "gravity").
        /// Populated automatically with every registered effect set to true on
        /// first launch - just flip the ones you don't want to `false`.
        /// </summary>
        [JsonPropertyName("Effects")]
        public Dictionary<string, bool> Effects { get; set; } = new();
    }
}
