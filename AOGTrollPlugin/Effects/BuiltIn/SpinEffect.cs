using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>Spins the target's view around continuously for a few seconds.</summary>
    public sealed class SpinEffect : ITrollEffect
    {
        private const float DurationSeconds = 4f;
        private const float TickInterval = 0.05f;
        private const float DegreesPerTick = 25f;

        public string Id => "spin";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.spin", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            ctx.PrintToTarget("troll.spin.chat");

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

                    QAngle current = pawn.EyeAngles;
                    QAngle spun = new(current.X, current.Y + DegreesPerTick, current.Z);
                    pawn.Teleport(null, spun, null);
                },
                TimerFlags.REPEAT
            );
        }
    }
}
