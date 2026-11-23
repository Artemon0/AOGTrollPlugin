using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class TeleportSpamEffect : ITrollEffect
    {
        private const float IntervalSeconds = 3f;
        private const float LandingHeightOffset = 32f;

        private readonly Dictionary<int, Timer> _timers = new();

        public string Id => "teleport_spam";
        public EffectCategory Category => EffectCategory.Movement;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.teleport_spam", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowSpamming = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (nowSpamming)
            {
                CCSPlayerController target = ctx.Target;

                if (_timers.Remove(target.Slot, out Timer? stale))
                    stale.Kill();

                TrollStateStore state = ctx.State;
                string id = Id;
                _timers[target.Slot] = ctx.Plugin.AddTimer(
                    IntervalSeconds,
                    () =>
                    {
                        if (!state.IsActive(id, target.Slot))
                            return;

                        TeleportToRandomPlayer(target);
                    },
                    TimerFlags.REPEAT
                );

                TeleportToRandomPlayer(target); // don't make them wait for the first jump
            }
            else
            {
                if (_timers.Remove(ctx.Target.Slot, out Timer? timer))
                    timer.Kill();
            }

            ctx.PrintToTarget(nowSpamming ? "troll.teleport_spam.on.chat" : "troll.teleport_spam.off.chat");
        }

        private static void TeleportToRandomPlayer(CCSPlayerController target)
        {
            CCSPlayerPawn? targetPawn = target.PlayerPawn?.Value;
            if (targetPawn is not { IsValid: true } || !target.PawnIsAlive)
                return;

            List<CCSPlayerController> candidates = Utilities.GetPlayers()
                .Where(p => p is { IsValid: true, PawnIsAlive: true } && p.Slot != target.Slot)
                .ToList();

            if (candidates.Count == 0)
                return;

            CCSPlayerController destination = candidates[Random.Shared.Next(candidates.Count)];
            Vector? destOrigin = destination.PlayerPawn?.Value?.AbsOrigin;
            if (destOrigin == null)
                return;

            Vector landing = new(destOrigin.X, destOrigin.Y, destOrigin.Z + LandingHeightOffset);
            targetPawn.Teleport(landing, null, null);
        }
    }
}
