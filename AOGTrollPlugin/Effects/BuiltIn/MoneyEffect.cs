using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Sets the target's money to a fixed amount. InGameMoneyServices.Account
    /// is the exact pattern CounterStrikeSharp's own TestPlugin uses to set
    /// player cash, so this one's on solid ground.
    /// </summary>
    public sealed class MoneyEffect : ITrollEffect
    {
        private const int BrokeAmount = 0;

        // CS2's default money cap is 16000 (mp_maxmoney); going well past it
        // is the classic "wait, that's not possible" prank. If your server
        // clamps to mp_maxmoney this will just cap there instead - that's
        // expected, not a bug.
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
