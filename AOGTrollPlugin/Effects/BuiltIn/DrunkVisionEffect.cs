using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Rapidly wobbles the target's FOV for a few seconds to give a
    /// "drunk"/seasick camera feeling, then restores the default FOV.
    /// NOTE: DesiredFOV is exposed by CounterStrikeSharp but its exact
    /// valid range can vary by game update - tune MinFov/MaxFov if it
    /// looks off on your server version.
    /// </summary>
    public sealed class DrunkVisionEffect : ITrollEffect
    {
        private const float DurationSeconds = 6f;
        private const float TickInterval = 0.15f;
        private const uint MinFov = 60;
        private const uint MaxFov = 110;
        private const uint DefaultFov = 90;

        public string Id => "drunk_vision";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.drunk_vision", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerController target = ctx.Target;
            if (!target.IsValid)
                return;

            ctx.PrintToTarget("troll.drunk_vision.chat");

            float elapsed = 0f;
            Timer? timer = null;
            timer = ctx.Plugin.AddTimer(
                TickInterval,
                () =>
                {
                    elapsed += TickInterval;

                    if (!target.IsValid || elapsed >= DurationSeconds)
                    {
                        if (target.IsValid)
                            target.DesiredFOV = DefaultFov;
                        timer?.Kill();
                        return;
                    }

                    target.DesiredFOV = (uint)Random.Shared.Next((int)MinFov, (int)MaxFov + 1);
                },
                TimerFlags.REPEAT
            );
        }
    }
}