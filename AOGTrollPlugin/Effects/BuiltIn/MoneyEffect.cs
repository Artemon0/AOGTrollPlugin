using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class MoneyEffect : ITrollEffect
    {
        private const int BrokeAmount = 0;

        private const int RichAmount = 60000;

        public string Id => "money";
        public EffectCategory Category => EffectCategory.Utility;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.money.broke", ctx => Apply(ctx, BrokeAmount, "troll.money.broke.chat")),
            new TrollEffectVariant("troll.money.rich", ctx => Apply(ctx, RichAmount, "troll.money.rich.chat"))
        ];

        private static void Apply(TrollEffectContext ctx, int amount, string chatKey)
        {
            CCSPlayerController_InGameMoneyServices? moneyServices = ctx.Target.InGameMoneyServices;
            if (moneyServices == null)
                return;

            moneyServices.Account = amount;
            ctx.PrintToTarget(chatKey);
        }
    }
}
