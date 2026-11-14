using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Toggle - locks the target's FOV zoomed in like a permanent sniper scope, on any weapon.</summary>
    public sealed class SniperVisionEffect : ITrollEffect
    {
        private const uint ZoomedFov = 20;
        private const uint DefaultFov = 90;

        public string Id => "sniper_vision";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.sniper_vision", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowZoomed = ctx.State.Toggle(Id, ctx.Target.Slot);
            ctx.Target.DesiredFOV = nowZoomed ? ZoomedFov : DefaultFov;
            ctx.PrintToTarget(nowZoomed ? "troll.sniper_vision.on.chat" : "troll.sniper_vision.off.chat");
        }
    }
}
