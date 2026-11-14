using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Streamer-prank follow-up to the "unbind a movement key" idea from
    /// earlier: real client-side unbinding isn't something a server can do
    /// (see BlockDefuseEffect's doc for why), but this gets close to the
    /// same felt effect for one direction at a time, entirely server-side.
    ///
    /// How it works: every tick, for each flagged target, if the chosen
    /// button (Forward/Back/Moveleft/Moveright) is currently held, the pawn
    /// gets teleported back to wherever it was the last tick that button
    /// WASN'T held. The result: as long as they hold that key, they don't
    /// go anywhere; every other key keeps working normally, and the moment
    /// they let go, movement resumes immediately with no server message.
    ///
    /// Known limitation: this corrects the pawn's full position, not just
    /// the movement component from the blocked key. If they hold the
    /// blocked key together with a working one (e.g. blocked Forward +
    /// working Moveright on a strafe-jump), the correction cancels both for
    /// that tick, so diagonal movement looks briefly glitchy rather than
    /// cleanly "one direction removed." Good enough for a prank, not a
    /// surgical instrument.
    /// </summary>
    public sealed class StuckKeyEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        // Target slot -> last known position while the blocked button was released.
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

            // Silent to the target, same as the other streamer pranks - see
            // the class doc for why.
            ctx.PrintToAdmin(nowStuck ? "troll.stuck_key.on.chat" : "troll.stuck_key.off.chat");
        }

        // Which specific button each stuck target has blocked - TrollStateStore
        // only tracks "is this effect on for this slot", not which of the four
        // button variants, so that part lives here instead.
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
