using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class HazyVisionEffect : ITrollEffect
    {
        private const float RefreshIntervalSeconds = 1f;
        private const float BlindWindowSeconds = 2.5f;
        private const float HazyAlpha = 120f;

        private readonly Dictionary<int, Timer> _timers = new();

        public string Id => "hazy_vision";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.hazy_vision", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowHazy = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (nowHazy)
            {
                CCSPlayerController target = ctx.Target;
                Apply(target);

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

                        Apply(target);
                    },
                    TimerFlags.REPEAT
                );
            }
            else
            {
                if (_timers.Remove(ctx.Target.Slot, out Timer? timer))
                    timer.Kill();

                Clear(ctx.Target);
            }

            ctx.PrintToTarget(nowHazy ? "troll.hazy_vision.on.chat" : "troll.hazy_vision.off.chat");
        }

        private static void Apply(CCSPlayerController target)
        {
            CCSPlayerPawn? pawn = target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !target.PawnIsAlive)
                return;

            float now = Server.CurrentTime;
            pawn.FlashDuration = BlindWindowSeconds;
            pawn.FlashMaxAlpha = HazyAlpha;
            pawn.BlindStartTime = now;
            pawn.BlindUntilTime = now + BlindWindowSeconds;

            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashDuration");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindStartTime");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindUntilTime");
        }

        private static void Clear(CCSPlayerController target)
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
