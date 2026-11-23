using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class GravityEffect : ITrollEffect
    {
        private const float LowGravityScale = 0.25f;
        private const float NormalGravityScale = 1.0f;

        public string Id => "gravity";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.gravity", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            bool nowOn = ctx.State.Toggle("gravity", ctx.Target.Slot);

            pawn.GravityScale = nowOn ? LowGravityScale : NormalGravityScale;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_flGravityScale");

            ctx.PrintToTarget(nowOn ? "troll.gravity.on.chat" : "troll.gravity.off.chat");
        }
    }
}
