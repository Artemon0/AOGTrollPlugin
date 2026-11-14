using System.Collections.Generic;

namespace AOGTrollPlugin.Effects
{
    /// <summary>
    /// Keeps track of which "toggle style" effects (noclip, gravity, freeze, ...)
    /// are currently active on which player slot. Effects that just fire once
    /// (kill, skybox, slap...) don't need this at all.
    /// </summary>
    public sealed class TrollStateStore
    {
        private readonly Dictionary<string, HashSet<int>> _active = new();

        /// <summary>Flips the state for (effectId, slot) and returns the new state.</summary>
        public bool Toggle(string effectId, int slot)
        {
            if (!_active.TryGetValue(effectId, out HashSet<int>? set))
            {
                set = [];
                _active[effectId] = set;
            }

            if (set.Remove(slot))
                return false;

            set.Add(slot);
            return true;
        }

        public bool IsActive(string effectId, int slot) =>
            _active.TryGetValue(effectId, out HashSet<int>? set) && set.Contains(slot);

        /// <summary>
        /// True if at least one player has this effect active right now. Cheap
        /// early-out for hooks that run every tick (e.g. CheckTransmit) so they
        /// don't do any per-player work in the common case where nobody has
        /// the effect toggled on.
        /// </summary>
        public bool HasActive(string effectId) =>
            _active.TryGetValue(effectId, out HashSet<int>? set) && set.Count > 0;

        public void Clear(string effectId, int slot)
        {
            if (_active.TryGetValue(effectId, out HashSet<int>? set))
                set.Remove(slot);
        }

        /// <summary>Used on round end / plugin unload to make sure nobody stays stuck.</summary>
        public void ClearAll() => _active.Clear();

        public IEnumerable<int> ActiveSlots(string effectId) =>
            _active.TryGetValue(effectId, out HashSet<int>? set) ? set : [];
    }
}
