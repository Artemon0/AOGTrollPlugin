using System;
using System.Collections.Generic;

namespace AOGTrollPlugin.Effects
{
    /// <summary>
    /// One selectable line in the effect's menu. Most effects have exactly one
    /// variant (e.g. "Skybox"); some need more than one (e.g. "Connection
    /// problems" -> "With chat" / "Silent").
    /// </summary>
    /// <param name="LabelLocalizationKey">Localization key shown as the menu label.</param>
    /// <param name="Run">Executed when the admin picks this variant.</param>
    public sealed record TrollEffectVariant(string LabelLocalizationKey, Action<TrollEffectContext> Run);

    /// <summary>
    /// A single troll effect. Implementations are stateless singletons that get
    /// registered once in <see cref="EffectRegistry"/> and reused for every
    /// invocation; any per-target state must live in <see cref="TrollStateStore"/>,
    /// not as instance fields on the effect itself.
    /// </summary>
    public interface ITrollEffect
    {
        /// <summary>
        /// Stable id used as the config key (Effects.{Id}) and as the base
        /// localization key (troll.{Id}). Keep it snake_case and never rename
        /// it once shipped, or existing configs will silently reset to default.
        /// </summary>
        string Id { get; }

        EffectCategory Category { get; }

        IReadOnlyList<TrollEffectVariant> GetVariants();
    }
}
