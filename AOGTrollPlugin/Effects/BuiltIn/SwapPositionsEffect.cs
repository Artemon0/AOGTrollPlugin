using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class SwapPositionsEffect : ITrollEffect
    {
        public string Id => "swap_positions";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.swap_positions", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? targetPawn = ctx.Target.PlayerPawn?.Value;
            Vector? targetOrigin = targetPawn?.AbsOrigin;
            if (targetPawn is not { IsValid: true } || !ctx.Target.PawnIsAlive || targetOrigin == null)
                return;

            List<CCSPlayerController> candidates = Utilities.GetPlayers()
                .Where(p => p is { IsValid: true, PawnIsAlive: true } && p.Slot != ctx.Target.Slot)
                .ToList();

            if (candidates.Count == 0)
                return;

            CCSPlayerController other = candidates[Random.Shared.Next(candidates.Count)];
            CCSPlayerPawn? otherPawn = other.PlayerPawn?.Value;
            Vector? otherOrigin = otherPawn?.AbsOrigin;
            if (otherPawn is not { IsValid: true } || otherOrigin == null)
                return;

            Vector targetGoesTo = new(otherOrigin.X, otherOrigin.Y, otherOrigin.Z);
            Vector otherGoesTo = new(targetOrigin.X, targetOrigin.Y, targetOrigin.Z);

            targetPawn.Teleport(targetGoesTo, null, null);
            otherPawn.Teleport(otherGoesTo, null, null);

            ctx.PrintToTarget("troll.swap_positions.chat");
        }
    }
}
