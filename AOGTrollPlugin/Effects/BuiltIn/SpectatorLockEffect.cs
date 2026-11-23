using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class SpectatorLockEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "spectator_lock";
        public EffectCategory Category => EffectCategory.AntiCheat;

        public SpectatorLockEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam, HookMode.Post);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.spectator_lock", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowLocked = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (nowLocked)
                ctx.Target.ChangeTeam(CsTeam.Spectator);

            ctx.PrintToTarget(nowLocked ? "troll.spectator_lock.on.chat" : "troll.spectator_lock.off.chat");
            ctx.PrintToAdmin(nowLocked ? "troll.spectator_lock.on.chat" : "troll.spectator_lock.off.chat");
        }

        private HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player is not { IsValid: true } || !_state.IsActive(Id, player.Slot))
                return HookResult.Continue;

            if ((CsTeam)@event.Team == CsTeam.Spectator)
                return HookResult.Continue;

            Server.NextFrame(() =>
            {
                if (player.IsValid)
                    player.ChangeTeam(CsTeam.Spectator);
            });

            return HookResult.Continue;
        }
    }
}
