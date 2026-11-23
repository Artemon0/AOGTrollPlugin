using System;
using System.Collections.Generic;
using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class TintEffect : ITrollEffect
    {
        private static readonly Color[] Palette =
        [
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 128, 255),
            Color.FromArgb(255, 255, 0, 255),
            Color.FromArgb(255, 255, 255, 0)
        ];

        public string Id => "tint";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.tint", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true })
                return;

            bool nowTinted = ctx.State.Toggle("tint", ctx.Target.Slot);

            pawn.Render = nowTinted ? Palette[Random.Shared.Next(Palette.Length)] : Color.FromArgb(255, 255, 255, 255);
            pawn.RenderMode = RenderMode_t.kRenderTransAlpha;
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

            ctx.PrintToTarget(nowTinted ? "troll.tint.on.chat" : "troll.tint.off.chat");
        }
    }
}
