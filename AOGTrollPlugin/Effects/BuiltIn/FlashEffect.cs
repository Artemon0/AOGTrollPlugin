using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class FlashEffect : ITrollEffect
    {
        private const float DurationSeconds = 4f;
        private const float MaxAlpha = 255f;

        public string Id => "flash";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.flash", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            float now = Server.CurrentTime;

            pawn.FlashDuration = DurationSeconds;
            pawn.FlashMaxAlpha = MaxAlpha;
            pawn.BlindStartTime = now;
            pawn.BlindUntilTime = now + DurationSeconds;

            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashDuration");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindStartTime");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_blindUntilTime");

            ctx.PrintToTarget("troll.flash.chat");
        }
    }
}
