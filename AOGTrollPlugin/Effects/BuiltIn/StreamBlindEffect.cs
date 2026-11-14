using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Streamer-prank effect: an indefinite, fully silent blind. Unlike
    /// FlashEffect (a one-shot cosmetic flash that fades on its own and
    /// prints a chat line), this one stays on until toggled off and never
    /// prints anything to the target - it's meant to look like their own
    /// game glitched, not like something a plugin did.
    ///
    /// Implementation note: the underlying blind fields decay on their own,
    /// so staying blind indefinitely means re-applying them on a repeating
    /// timer rather than setting them once. The refresh window is longer
    /// than the timer interval so there's no flicker back to clear vision
    /// between refreshes.
    ///
    /// Like ConnectionProblemsEffect, this effect keeps its own per-slot
    /// timer dictionary alongside TrollStateStore's on/off flag, since the
    /// timer handle itself isn't state TrollStateStore is designed to hold.
    /// </summary>
    public sealed class StreamBlindEffect : ITrollEffect
    {
        private const float RefreshIntervalSeconds = 1f;
        private const float BlindWindowSeconds = 2.5f;
        private const float MaxAlpha = 255f;

        private readonly Dictionary<int, Timer> _timers = new();

        public string Id => "stream_blind";
        public EffectCategory Category => EffectCategory.Streamer;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.stream_blind", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowBlind = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (nowBlind)
            {
                CCSPlayerController target = ctx.Target;
                ApplyBlind(target);

                if (_timers.Remove(target.Slot, out Timer? stale))
                    stale.Kill();

                TrollStateStore state = ctx.State;
                string id = Id;
                _timers[target.Slot] = ctx.Plugin.AddTimer(
                    RefreshIntervalSeconds,
                    () =>
                    {
                        if (!state.IsActive(id, target.Slot))
                            return;

                        ApplyBlind(target);
                    },
                    TimerFlags.REPEAT
                );
            }
            else
            {
                if (_timers.Remove(ctx.Target.Slot, out Timer? timer))
                    timer.Kill();

                ClearBlind(ctx.Target);
            }

            // Admin-only - see class doc for why the target hears nothing.
            ctx.PrintToAdmin(nowBlind ? "troll.stream_blind.on.chat" : "troll.stream_blind.off.chat");
        }

        private static void ApplyBlind(CCSPlayerController target)
        {
            CCSPlayerPawn? pawn = target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !target.PawnIsAlive)
                return;

            float now = Server.CurrentTime;
            pawn.FlashDuration = BlindWindowSeconds;
            pawn.FlashMaxAlpha = MaxAlpha;
            pawn.BlindStartTime = now;
            pawn.BlindUntilTime = now + BlindWindowSeconds;

            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashDuration");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindStartTime");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindUntilTime");
        }

        private static void ClearBlind(CCSPlayerController target)
        {
            CCSPlayerPawn? pawn = target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            pawn.FlashMaxAlpha = 0f;
            pawn.BlindUntilTime = Server.CurrentTime;

            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindUntilTime");
        }
    }
}
