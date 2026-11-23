using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.ValveConstants.Protobuf;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class DecoyBotEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

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

            Server.ExecuteCommand(targetIsCt ? "bot_add_t" : "bot_add_ct");

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

                    if (viewer.Slot == entry.Key || viewer.Slot == entry.Value.AdminSlot)
                        continue;

                    info.TransmitEntities.Remove(decoyPawn);
                }
            }
        }
    }
}