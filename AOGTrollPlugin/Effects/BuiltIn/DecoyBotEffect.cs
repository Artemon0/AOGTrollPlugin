using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.ValveConstants.Protobuf;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Anti-cheat bait effect: spawns a real bot on the opposite team and
    /// hides it from everyone's network stream except the flagged target's -
    /// and the admin's, who needs to be able to see it too, for reasons
    /// explained below. Because it's an actual bot, the game's own AI moves
    /// it, shoots with it, and gives it a real gun - nothing here is a
    /// scripted fake. If the target's aim snaps to it through smoke or a
    /// wall the moment it's the only "enemy" they could possibly be
    /// tracking, that's a real, hard-to-explain-away signal.
    ///
    /// One thing I could NOT deliver as asked: a visual outline that renders
    /// only on the admin's screen so the admin can tell the decoy apart from
    /// a real player at a glance. Per-observer glow/outline rendering isn't
    /// something CounterStrikeSharp's public API exposes a confirmed way to
    /// do (the game's own "spotted" glow is per-team, not per-player, so it
    /// can't be aimed at one specific viewer). What this does instead: the
    /// admin who triggers the effect gets a chat line naming exactly which
    /// bot was just spawned as the decoy, and is the only other person besides
    /// the target who can see it at all - so there's no ambiguity about which
    /// one it is once you're looking at your own screen. The bot's in-game
    /// name is left untouched on purpose - renaming it would be visible to
    /// the target too (name is a shared networked field, there's no
    /// per-observer version of it either) and would immediately give the
    /// whole thing away.
    ///
    /// Gameplay side effects worth knowing before you use this mid-round:
    /// it's a real player slot, so it counts toward round-win conditions,
    /// can plant/defuse, and will show up in round-end bot logic like any
    /// other bot your server would normally add. Consider your `bot_difficulty`
    /// setting - a decoy that plays badly is also a decoy that's easy to
    /// dismiss as "just a bad bot" if the target does clock it visually.
    /// </summary>
    public sealed class DecoyBotEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        // Target slot -> (decoy bot's controller slot, admin slot allowed to see it).
        private readonly Dictionary<int, (int DecoySlot, int AdminSlot)> _decoys = new();

        public string Id => "decoy_bot";
        public EffectCategory Category => EffectCategory.AntiCheat;

        public DecoyBotEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterListener<Listeners.CheckTransmit>(OnCheckTransmit);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.decoy_bot", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool turningOn = ctx.State.Toggle(Id, ctx.Target.Slot);

            if (!turningOn)
            {
                RemoveDecoy(ctx.Target.Slot);
                ctx.PrintToAdmin("troll.decoy_bot.off.chat");
                return;
            }

            SpawnDecoy(ctx);
        }

        private void SpawnDecoy(TrollEffectContext ctx)
        {
            int targetSlot = ctx.Target.Slot;
            int adminSlot = ctx.Admin.Slot;
            bool targetIsCt = ctx.Target.Team == CsTeam.CounterTerrorist;

            HashSet<int> botSlotsBefore = Utilities.GetPlayers()
                .Where(p => p is { IsValid: true, IsBot: true })
                .Select(p => p.Slot)
                .ToHashSet();

            // Spawn on the opposite team so it reads as a normal enemy target.
            Server.ExecuteCommand(targetIsCt ? "bot_add_t" : "bot_add_ct");

            // Bots take a frame or two to actually get a controller + pawn,
            // so give it half a second before we go looking for it.
            ctx.Plugin.AddTimer(0.5f, () =>
            {
                CCSPlayerController? decoy = Utilities.GetPlayers()
                    .FirstOrDefault(p => p is { IsValid: true, IsBot: true } && !botSlotsBefore.Contains(p.Slot));

                if (decoy is not { IsValid: true })
                {
                    ctx.PrintToAdmin("troll.decoy_bot.spawn_failed.chat");
                    ctx.State.Toggle(Id, targetSlot); // roll the toggle back off
                    return;
                }

                // NOTE: GiveNamedItem is a very commonly used CS2Sharp pawn
                // method, but double-check it against your installed
                // CounterStrikeSharp.API version if this doesn't compile.
                string weaponClass = targetIsCt ? "weapon_ak47" : "weapon_m4a1";
                string pistolClass = targetIsCt ? "weapon_usp_silencer" : "weapon_glock";
                decoy.GiveNamedItem(pistolClass);
                decoy.GiveNamedItem(weaponClass);


                _decoys[targetSlot] = (decoy.Slot, adminSlot);
                ctx.PrintToAdmin("troll.decoy_bot.on.chat");
            });
        }

        private void RemoveDecoy(int targetSlot)
        {
            if (!_decoys.Remove(targetSlot, out (int DecoySlot, int AdminSlot) info))
                return;

            CCSPlayerController? decoy = Utilities.GetPlayers()
                .FirstOrDefault(p => p.IsValid && p.Slot == info.DecoySlot);

            decoy?.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED);
        }

        private void OnCheckTransmit(CCheckTransmitInfoList infoList)
        {
            if (_decoys.Count == 0)
                return;

            List<CCSPlayerController> allPlayers = Utilities.GetPlayers();

            foreach (KeyValuePair<int, (int DecoySlot, int AdminSlot)> entry in _decoys)
            {
                CCSPlayerPawn? decoyPawn = allPlayers
                    .FirstOrDefault(p => p.IsValid && p.Slot == entry.Value.DecoySlot)
                    ?.PlayerPawn?.Value;

                if (decoyPawn is not { IsValid: true })
                    continue;

                foreach ((CCheckTransmitInfo info, CCSPlayerController? viewer) in infoList)
                {
                    if (viewer is not { IsValid: true })
                        continue;

                    // Only the flagged target and the admin who spawned this
                    // specific decoy are allowed to see it.
                    if (viewer.Slot == entry.Key || viewer.Slot == entry.Value.AdminSlot)
                        continue;

                    info.TransmitEntities.Remove(decoyPawn);
                }
            }
        }
    }
}