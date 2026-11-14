using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Cosmetic instant flashbang - whites out the target's screen for a few
    /// seconds using the same fields the game itself sets when a real
    /// flashbang detonates nearby. NOTE: like IgniteEffect, this relies on
    /// CounterStrikeSharp's schema access to CCSPlayerPawnBase's flash
    /// fields (FlashDuration, FlashMaxAlpha, BlindStartTime, BlindUntilTime);
    /// re-check those against CounterStrikeSharp's docs if a game update
    /// ever makes this stop working.
    /// </summary>
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
