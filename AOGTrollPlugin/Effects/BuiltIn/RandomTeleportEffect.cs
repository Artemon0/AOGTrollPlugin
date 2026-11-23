using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class RandomTeleportEffect : ITrollEffect
    {
        private const float LandingHeightOffset = 32f;

        public string Id => "random_teleport";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.random_teleport", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? targetPawn = ctx.Target.PlayerPawn?.Value;
            if (targetPawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            List<CCSPlayerController> candidates = Utilities.GetPlayers()
                .Where(p => p is { IsValid: true, PawnIsAlive: true } && p.Slot != ctx.Target.Slot)
                .ToList();

            if (candidates.Count == 0)
                return;

            CCSPlayerController destination = candidates[Random.Shared.Next(candidates.Count)];
            Vector? destOrigin = destination.PlayerPawn?.Value?.AbsOrigin;
            if (destOrigin == null)
                return;

            Vector landing = new(destOrigin.X, destOrigin.Y, destOrigin.Z + LandingHeightOffset);
            targetPawn.Teleport(landing, null, null);

            ctx.PrintToTarget("troll.random_teleport.chat");
        }
    }
}
