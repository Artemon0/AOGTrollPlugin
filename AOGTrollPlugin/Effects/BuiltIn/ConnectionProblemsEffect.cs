using System;
using System.Collections.Generic;
using System.Numerics;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;


namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Simulates heavy desync/jitter by rapidly nudging the target to random
    /// nearby positions. Ends automatically after <see cref="DurationSeconds"/>.
    /// </summary>
    public sealed class ConnectionProblemsEffect : ITrollEffect
    {
        private const float DurationSeconds = 5f;
        private const float TickInterval = 0.2f;
        private const float JitterRange = 10f;

        // Keyed by player slot. Only this effect touches this dictionary, so a
        // plain instance field (rather than TrollStateStore) is simplest here -
        // we need to store the actual Timer, not just an on/off flag.
        private readonly Dictionary<int, Timer> _running = new();

        public string Id => "connection_problems";
        public EffectCategory Category => EffectCategory.Connection;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.connection_problems.with_chat", ctx => Start(ctx, silent: false)),
            new TrollEffectVariant("troll.connection_problems.silent", ctx => Start(ctx, silent: true))
        ];

        private void Start(TrollEffectContext ctx, bool silent)
        {
            CCSPlayerController target = ctx.Target;
            if (!target.IsValid || !target.PawnIsAlive)
                return;

            // Restart cleanly if it's already running on this target.
            if (_running.TryGetValue(target.Slot, out Timer? existing))
            {
                existing.Kill();
                _running.Remove(target.Slot);
            }

            if (!silent)
                ctx.PrintToTarget("troll.connection_problems.chat");

            float elapsed = 0f;
            int slot = target.Slot;

            Timer lagTimer = ctx.Plugin.AddTimer(
                TickInterval,
                () =>
                {
                    elapsed += TickInterval;

                    CCSPlayerPawn? pawn = target.IsValid ? target.PlayerPawn.Value : null;
                    bool stillAlive = target is { IsValid: true, PawnIsAlive: true } && pawn is { IsValid: true } &&
                                      pawn.AbsOrigin != null;

                    if (!stillAlive || elapsed >= DurationSeconds)
                    {
                        if (_running.TryGetValue(slot, out Timer? t))
                        {
                            t.Kill();
                            _running.Remove(slot);
                        }

                        if (stillAlive && !silent)
                            ctx.PrintToTarget("troll.connection_problems.chat.off");

                        return;
                    }

                    Vector origin = pawn!.AbsOrigin!;
                    pawn.Teleport(
                        new Vector(
                            origin.X + (Random.Shared.NextSingle() - 0.5f) * JitterRange,
                            origin.Y + (Random.Shared.NextSingle() - 0.5f) * JitterRange,
                            origin.Z
                        ),
                        null,
                        null
                    );
                },
                TimerFlags.REPEAT
            );

            _running[slot] = lagTimer;
        }

        /// <summary>Called from plugin Unload() so no timers survive a hot reload.</summary>
        public void StopAll()
        {
            foreach (Timer timer in _running.Values)
                timer.Kill();
            _running.Clear();
        }
    }
}