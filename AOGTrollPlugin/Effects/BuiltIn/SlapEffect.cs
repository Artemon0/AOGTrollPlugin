using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Classic admin "slap" - launches the target with a random impulse.</summary>
    public sealed class SlapEffect : ITrollEffect
    {
        private const float HorizontalForce = 250f;
        private const float VerticalForce = 200f;

        public string Id => "slap";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.slap", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            Vector current = pawn.AbsVelocity;
            Vector impulse = new(
                current.X + (Random.Shared.NextSingle() - 0.5f) * 2f * HorizontalForce,
                current.Y + (Random.Shared.NextSingle() - 0.5f) * 2f * HorizontalForce,
                current.Z + VerticalForce
            );

            pawn.Teleport(null, null, impulse);
            ctx.PrintToTarget("troll.slap.chat");
        }
    }
}
