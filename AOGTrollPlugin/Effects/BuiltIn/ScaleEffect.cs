using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Scales the target's model up (Giant) or down (Tiny) via the scene
    /// node's AbsScale, with a third variant to reset it back to normal.
    ///
    /// Known caveat: this only changes the visual model size. Source 2's
    /// hitbox/collision volumes do NOT scale along with it, so a "giant"
    /// player still has a normal-sized hitbox floating in the middle of
    /// their oversized model, and a "tiny" player is still exactly as easy
    /// (or hard) to hit as before. This is a purely cosmetic effect.
    ///
    /// Second caveat, this one about the code itself: AbsScale lives on
    /// CGameSceneNode, which is a nested object reached through the pawn's
    /// CBodyComponent rather than a field declared directly on the pawn's
    /// own schema chain (unlike e.g. FlashEffect's fields). I'm confident
    /// the AbsScale field itself is real, less confident that
    /// `SetStateChanged(pawn, "CGameSceneNode", ...)` is the exactly correct
    /// way to flag a nested field like this dirty for networking - test it
    /// after building. If the scale change doesn't show up on other clients,
    /// try `SetStateChanged(pawn, "CBodyComponent", "m_pSceneNode")` instead
    /// (marking the parent link dirty rather than the leaf field).
    /// </summary>
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
