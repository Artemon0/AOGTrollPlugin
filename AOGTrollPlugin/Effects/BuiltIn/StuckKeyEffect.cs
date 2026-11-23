using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class StuckKeyEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        private readonly Dictionary<int, Vector> _lastFreePosition = new();

        public string Id => "stuck_key";
        public EffectCategory Category => EffectCategory.Streamer;

        public StuckKeyEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterListener<Listeners.OnTick>(OnTick);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.stuck_key.forward", ctx => Toggle(ctx, PlayerButtons.Forward)),
            new TrollEffectVariant("troll.stuck_key.back", ctx => Toggle(ctx, PlayerButtons.Back)),
            new TrollEffectVariant("troll.stuck_key.left", ctx => Toggle(ctx, PlayerButtons.Moveleft)),
            new TrollEffectVariant("troll.stuck_key.right", ctx => Toggle(ctx, PlayerButtons.Moveright))
        ];

        private void Toggle(TrollEffectContext ctx, PlayerButtons button)
        {
            bool nowStuck = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (nowStuck)
                _stuckButtonBySlot[ctx.Target.Slot] = button;
            else
                _stuckButtonBySlot.Remove(ctx.Target.Slot);

            _lastFreePosition.Remove(ctx.Target.Slot);

            ctx.PrintToAdmin(nowStuck ? "troll.stuck_key.on.chat" : "troll.stuck_key.off.chat");
        }

        private readonly Dictionary<int, PlayerButtons> _stuckButtonBySlot = new();

        private void OnTick()
        {
            if (_stuckButtonBySlot.Count == 0)
                return;

            List<CCSPlayerController> allPlayers = Utilities.GetPlayers();

            foreach (KeyValuePair<int, PlayerButtons> entry in _stuckButtonBySlot)
            {
                if (!_state.IsActive(Id, entry.Key))
                    continue;

                CCSPlayerController? player = allPlayers.FirstOrDefault(p => p.IsValid && p.Slot == entry.Key);
                CCSPlayerPawn? pawn = player?.PlayerPawn?.Value;
                if (player is not { IsValid: true, PawnIsAlive: true } || pawn is not { IsValid: true })
                    continue;

                Vector? currentOrigin = pawn.AbsOrigin;
                if (currentOrigin == null)
                    continue;

                bool blockedButtonHeld = player.Buttons.HasFlag(entry.Value);

                if (blockedButtonHeld && _lastFreePosition.TryGetValue(entry.Key, out Vector? lastFree) && lastFree != null)
                {
                    pawn.Teleport(lastFree, null, new Vector(0, 0, 0));
                }
                else if (!blockedButtonHeld)
                {
                    _lastFreePosition[entry.Key] = new Vector(currentOrigin.X, currentOrigin.Y, currentOrigin.Z);
                }
            }
        }
    }
}
