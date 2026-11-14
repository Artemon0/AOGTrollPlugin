using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class SkyboxEffect : ITrollEffect
    {
        private const float SkyboxTeleportOffset = 1000.0f;

        public string Id => "skybox";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.skybox", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            Vector? currentPos = pawn.AbsOrigin;
            if (currentPos != null)
            {
                Vector newPos = new(currentPos.X, currentPos.Y, currentPos.Z + SkyboxTeleportOffset);
                pawn.Teleport(newPos, null, null);
            }

            ctx.PrintToTarget("troll.skybox.chat");
        }
    }
}
