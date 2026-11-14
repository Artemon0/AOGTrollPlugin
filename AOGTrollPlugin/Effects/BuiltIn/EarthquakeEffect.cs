using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Shakes the target's screen by jittering their view angles briefly.</summary>
    public sealed class EarthquakeEffect : ITrollEffect
    {
        private const float DurationSeconds = 3f;
        private const float TickInterval = 0.05f;
        private const float ShakeDegrees = 4f;

        public string Id => "earthquake";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.earthquake", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            ctx.PrintToTarget("troll.earthquake.chat");

            float elapsed = 0f;
            Timer? timer = null;
            timer = ctx.Plugin.AddTimer(
                TickInterval,
                () =>
                {
                    elapsed += TickInterval;

                    if (!ctx.Target.IsValid || !ctx.Target.PawnIsAlive || pawn is not { IsValid: true } || elapsed >= DurationSeconds)
                    {
                        timer?.Kill();
                        return;
                    }

                    QAngle baseAngles = pawn.EyeAngles;
                    QAngle shaken = new(
                        baseAngles.X + (Random.Shared.NextSingle() - 0.5f) * ShakeDegrees,
                        baseAngles.Y + (Random.Shared.NextSingle() - 0.5f) * ShakeDegrees,
                        baseAngles.Z
                    );
                    pawn.Teleport(null, shaken, null);
                },
                TimerFlags.REPEAT
            );
        }
    }
}
