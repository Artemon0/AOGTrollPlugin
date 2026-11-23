using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
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

            ctx.PrintToAdmin(nowBlind ? "troll.blind_enemies.on.chat" : "troll.blind_enemies.off.chat");
        }

        private void OnCheckTransmit(CCheckTransmitInfoList infoList)
        {
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
