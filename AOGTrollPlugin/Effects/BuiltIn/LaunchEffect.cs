using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class LaunchEffect : ITrollEffect
    {
        private const float UpwardVelocity = 700f;
        private const float HorizontalKick = 200f;

        public string Id => "launch";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.launch", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            Vector velocity = new(
                (float)((Random.Shared.NextDouble() * 2 - 1) * HorizontalKick),
                (float)((Random.Shared.NextDouble() * 2 - 1) * HorizontalKick),
                UpwardVelocity
            );

            pawn.Teleport(null, null, velocity);
            ctx.PrintToTarget("troll.launch.chat");
        }
    }
}
