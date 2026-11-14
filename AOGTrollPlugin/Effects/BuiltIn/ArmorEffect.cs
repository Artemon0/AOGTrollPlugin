using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Sets the target's armor to full or strips it entirely.</summary>
    public sealed class ArmorEffect : ITrollEffect
    {
        private const int FullArmor = 100;
        private const int NoArmor = 0;

        public string Id => "armor";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.armor.full", ctx => Apply(ctx, FullArmor, "troll.armor.full.chat")),
            new TrollEffectVariant("troll.armor.strip", ctx => Apply(ctx, NoArmor, "troll.armor.strip.chat"))
        ];

        private static void Apply(TrollEffectContext ctx, int amount, string chatKey)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn.Value;
            if (pawn is not { IsValid: true })
                return;

            pawn.ArmorValue = amount;
            ctx.PrintToTarget(chatKey);
        }
    }
}
