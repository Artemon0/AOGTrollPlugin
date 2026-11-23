using System.Collections.Generic;
using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class InvisibleEffect : ITrollEffect
    {
        public string Id => "invisible";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.invisible", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            bool nowInvisible = ctx.State.Toggle("invisible", ctx.Target.Slot);

            pawn.Render = nowInvisible ? Color.FromArgb(0, 255, 255, 255) : Color.FromArgb(255, 255, 255, 255);
            pawn.RenderMode = RenderMode_t.kRenderTransAlpha;
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

            ctx.PrintToTarget(nowInvisible ? "troll.invisible.on.chat" : "troll.invisible.off.chat");
        }
    }
}
