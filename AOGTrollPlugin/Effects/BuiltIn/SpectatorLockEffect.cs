using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Anti-cheat punishment effect: forces the target onto the spectator
    /// team and keeps them there. Any attempt to leave spectator - the
    /// team-select menu, the jointeam console command, auto-balance,
    /// anything at all that ends up firing a player_team event - gets
    /// reverted straight back to Spectator on the next server frame.
    ///
    /// Limits worth knowing: this is per-session state, not persisted
    /// anywhere, so it does not survive the target disconnecting and
    /// reconnecting (there's no way to prevent an actual disconnect from
    /// this plugin, or any plugin). It's a "can't play while connected"
    /// lock, not a ban.
    /// </summary>
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

            // Don't call ChangeTeam synchronously from inside the event that
            // just fired because of a team change - push the revert to the
            // next server frame instead.
            Server.NextFrame(() =>
            {
                if (player.IsValid)
                    player.ChangeTeam(CsTeam.Spectator);
            });

            return HookResult.Continue;
        }
    }
}
