using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace AOGTrollPlugin.Effects.BuiltIn
{
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
