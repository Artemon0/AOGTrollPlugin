using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class NoclipEffect : ITrollEffect
    {
        public string Id => "noclip";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.noclip", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CBasePlayerPawn? pawn = ctx.Target.Pawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            bool nowOn = ctx.State.Toggle("noclip", ctx.Target.Slot);

            pawn.MoveType = nowOn ? MoveType_t.MOVETYPE_NOCLIP : MoveType_t.MOVETYPE_WALK;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");

            ctx.PrintToTarget(nowOn ? "troll.noclipon.chat" : "troll.noclipoff.chat");
        }
    }
}
