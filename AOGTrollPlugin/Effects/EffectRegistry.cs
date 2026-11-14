using System.Collections.Generic;
using System.Linq;

namespace AOGTrollPlugin.Effects
{
    /// <summary>
    /// Holds every known effect and answers "which ones are enabled right now"
    /// based on the plugin config. Adding a new effect to the plugin is just:
    /// write a class implementing <see cref="ITrollEffect"/>, call Register() on it
    /// in the plugin's Load(), add its lang keys. Nothing else needs to change.
    /// </summary>
    public sealed class EffectRegistry
    {
        private readonly List<ITrollEffect> _effects = [];

        public IReadOnlyList<ITrollEffect> All => _effects;

        public void Register(ITrollEffect effect) => _effects.Add(effect);

        public IEnumerable<ITrollEffect> Enabled(AOGTrollConfig config) =>
            _effects.Where(e => !config.Effects.TryGetValue(e.Id, out bool enabled) || enabled);

        public IEnumerable<IGrouping<EffectCategory, ITrollEffect>> EnabledByCategory(AOGTrollConfig config) =>
            Enabled(config).GroupBy(e => e.Category);

        /// <summary>
        /// Makes sure the config has an explicit entry for every registered
        /// effect (defaulting to enabled) so the generated config.json is
        /// self-documenting and admins can see every toggle without guessing ids.
        /// </summary>
        public void EnsureConfigDefaults(AOGTrollConfig config)
        {
            foreach (ITrollEffect effect in _effects)
                config.Effects.TryAdd(effect.Id, true);
        }
    }
}
