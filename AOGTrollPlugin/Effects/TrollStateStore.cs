using System.Collections.Generic;

namespace AOGTrollPlugin.Effects
{
    public sealed class TrollStateStore
    {
        private readonly Dictionary<string, HashSet<int>> _active = new();

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

        public bool HasActive(string effectId) =>
            _active.TryGetValue(effectId, out HashSet<int>? set) && set.Count > 0;

        public void Clear(string effectId, int slot)
        {
            if (_active.TryGetValue(effectId, out HashSet<int>? set))
                set.Remove(slot);
        }

        public void ClearAll() => _active.Clear();

        public IEnumerable<int> ActiveSlots(string effectId) =>
            _active.TryGetValue(effectId, out HashSet<int>? set) ? set : [];
    }
}
