using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class GodmodeEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "godmode";
        public EffectCategory Category => EffectCategory.Combat;

        public GodmodeEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt, HookMode.Post);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.godmode", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowGod = ctx.State.Toggle(Id, ctx.Target.Slot);
            ctx.PrintToTarget(nowGod ? "troll.godmode.on.chat" : "troll.godmode.off.chat");
        }

        private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player is not { IsValid: true } || !_state.IsActive(Id, player.Slot))
                return HookResult.Continue;

            CCSPlayerPawn? pawn = player.PlayerPawn?.Value;
            if (pawn is { IsValid: true })
                pawn.Health = pawn.MaxHealth > 0 ? pawn.MaxHealth : 100;

            return HookResult.Continue;
        }
    }
}
