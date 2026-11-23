using System.Collections.Generic;
using System.Linq;

namespace AOGTrollPlugin.Effects
{
    public sealed class EffectRegistry
    {
        private readonly List<ITrollEffect> _effects = [];

        public IReadOnlyList<ITrollEffect> All => _effects;

        public void Register(ITrollEffect effect) => _effects.Add(effect);

        public IEnumerable<ITrollEffect> Enabled(AOGTrollConfig config) =>
            _effects.Where(e => IsEnabled(e, config));

        public static bool IsEnabled(ITrollEffect effect, AOGTrollConfig config) =>
            !config.Effects.TryGetValue(effect.Id, out bool enabled) || enabled;

        public IEnumerable<IGrouping<EffectCategory, ITrollEffect>> EnabledByCategory(AOGTrollConfig config) =>
            Enabled(config).GroupBy(e => e.Category);

        public void EnsureConfigDefaults(AOGTrollConfig config)
        {
            foreach (ITrollEffect effect in _effects)
                config.Effects.TryAdd(effect.Id, true);
        }
    }
}
