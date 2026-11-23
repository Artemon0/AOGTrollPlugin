using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class OneHpEffect : ITrollEffect
    {
        public string Id => "one_hp";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.one_hp", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            pawn.Health = 1;
            ctx.PrintToTarget("troll.one_hp.chat");
        }
    }
}
