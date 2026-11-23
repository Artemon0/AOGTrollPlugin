using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class ScaleEffect : ITrollEffect
    {
        private const float GiantScale = 3.0f;
        private const float TinyScale = 0.3f;
        private const float NormalScale = 1.0f;

        public string Id => "scale";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.scale.giant", ctx => Apply(ctx, GiantScale, "troll.scale.giant.chat")),
            new TrollEffectVariant("troll.scale.tiny", ctx => Apply(ctx, TinyScale, "troll.scale.tiny.chat")),
            new TrollEffectVariant("troll.scale.normal", ctx => Apply(ctx, NormalScale, "troll.scale.normal.chat"))
        ];

        private static void Apply(TrollEffectContext ctx, float scale, string chatKey)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            CGameSceneNode? sceneNode = pawn?.CBodyComponent?.SceneNode;

            if (pawn is not { IsValid: true } || sceneNode == null)
                return;

            sceneNode.AbsScale = scale;
            Utilities.SetStateChanged(pawn, "CGameSceneNode", "m_flAbsScale");

            ctx.PrintToTarget(chatKey);
        }
    }
}
