using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Freezes the target in place (movement locked) until toggled off.</summary>
    public sealed class FreezeEffect : ITrollEffect
    {
        public string Id => "freeze";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.freeze", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CBasePlayerPawn? pawn = ctx.Target.Pawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            bool nowFrozen = ctx.State.Toggle("freeze", ctx.Target.Slot);

            pawn.MoveType = nowFrozen ? MoveType_t.MOVETYPE_NONE : MoveType_t.MOVETYPE_WALK;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");

            ctx.PrintToTarget(nowFrozen ? "troll.freeze.on.chat" : "troll.freeze.off.chat");
        }
    }
}
