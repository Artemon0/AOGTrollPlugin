using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class UpsideDownEffect : ITrollEffect
    {
        public string Id => "upside_down";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.upside_down", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            bool nowFlipped = ctx.State.Toggle(Id, ctx.Target.Slot);
            QAngle current = pawn.EyeAngles;
            QAngle flipped = new(current.X, current.Y, nowFlipped ? 180f : 0f);
            pawn.Teleport(null, flipped, null);

            ctx.PrintToTarget(nowFlipped ? "troll.upside_down.on.chat" : "troll.upside_down.off.chat");
        }
    }
}
