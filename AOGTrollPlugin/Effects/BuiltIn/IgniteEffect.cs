using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Sets the target on fire for a few seconds (cosmetic - low burn damage,
    /// won't drop them below 1 HP so it can't be used as a disguised kill).
    /// </summary>
    public sealed class IgniteEffect : ITrollEffect
    {
        private const float DurationSeconds = 4f;

        public string Id => "ignite";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.ignite", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            pawn.AcceptInput("Ignite");
            ctx.Plugin.AddTimer(DurationSeconds, () =>
            {
                if (pawn.IsValid) pawn.AcceptInput("Extinguish");
            });

            ctx.PrintToTarget("troll.ignite.chat");
        }
    }
}