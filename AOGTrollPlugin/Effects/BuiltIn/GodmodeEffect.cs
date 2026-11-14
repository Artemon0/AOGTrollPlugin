using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Toggle - makes the target effectively unkillable by restoring their
    /// health to whatever it was right before every hit, every time.
    /// Implemented as a Post hook on player_hurt rather than by blocking
    /// damage outright, since that keeps hit sounds/blood/hitmarkers working
    /// normally for whoever's shooting - only the actual health loss gets
    /// undone a moment later.
    /// </summary>
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
