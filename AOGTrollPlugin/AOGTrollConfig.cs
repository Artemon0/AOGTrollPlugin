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

        [JsonPropertyName("SoundEffectEvent")]
        public string SoundEffectEvent { get; set; } = "UI.WeaponPurchase";

        [JsonPropertyName("DiscordWebhookUrls")]
        public string[] DiscordWebhookUrls { get; set; } = [];

        [JsonPropertyName("Effects")]
        public Dictionary<string, bool> Effects { get; set; } = new();
    }
}
