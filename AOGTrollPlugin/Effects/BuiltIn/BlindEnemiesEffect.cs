using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Anti-cheat punishment effect: stops enemy player pawns from being
    /// networked to the target's own client at all, while the target keeps
    /// being sent to everyone else exactly as normal. Teammates stay visible
    /// to the target too - only the enemy team is stripped from their feed.
    ///
    /// This works by hooking CheckTransmit and removing every enemy pawn from
    /// the target's per-tick transmit list before it's sent. Since it acts on
    /// the actual network stream, it defeats any cheat that reads its data
    /// from the same source the game client itself renders from - which is
    /// how the overwhelming majority of public ESP/wallhack/radar cheats
    /// work. It does nothing for a cheat that gets positions from somewhere
    /// external to this game connection (a second PC spectating a stream, a
    /// radar fed by another source, etc.) - there's no server-side fix for
    /// that class of cheat.
    ///
    /// Deliberately silent: unlike every other effect here, this one never
    /// prints anything to the target, only to the admin who toggled it. The
    /// whole point of using this on a suspected cheater is that they
    /// shouldn't be able to tell whether their ESP just stopped working
    /// because of this or because of a bug in their own cheat - telling them
    /// outright just gets it disabled seconds later.
    /// </summary>
    public sealed class BlindEnemiesEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "blind_enemies";
        public EffectCategory Category => EffectCategory.AntiCheat;

        public BlindEnemiesEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterListener<Listeners.CheckTransmit>(OnCheckTransmit);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.blind_enemies", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowBlind = ctx.State.Toggle(Id, ctx.Target.Slot);

            // Admin-only feedback - see the class doc for why the target
            // never hears about this one.
            ctx.PrintToAdmin(nowBlind ? "troll.blind_enemies.on.chat" : "troll.blind_enemies.off.chat");
        }

        private void OnCheckTransmit(CCheckTransmitInfoList infoList)
        {
            // Cheap early-out: this hook fires every server tick for every
            // connected client, so skip all the per-player work unless
            // someone actually has the effect active right now.
            if (!_state.HasActive(Id))
                return;

            List<CCSPlayerController> allPlayers = Utilities.GetPlayers();

            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                if (player is not { IsValid: true } || !_state.IsActive(Id, player.Slot))
                    continue;

                foreach (CCSPlayerController other in allPlayers)
                {
                    if (other.Slot == player.Slot || other.Team == player.Team)
                        continue;

                    CCSPlayerPawn? enemyPawn = other.PlayerPawn.Value;
                    if (enemyPawn is { IsValid: true })
                        info.TransmitEntities.Remove(enemyPawn);
                }
            }
        }
    }
}
