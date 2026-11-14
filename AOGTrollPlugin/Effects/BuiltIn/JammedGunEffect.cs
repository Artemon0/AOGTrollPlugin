using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Toggle - zeroes the target's clip after every shot via a
    /// weapon_fire Post-hook, forcing a reload no matter how much ammo
    /// they actually had left. Feels like the gun jams constantly.
    /// </summary>
    public sealed class JammedGunEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "jammed_gun";
        public EffectCategory Category => EffectCategory.Combat;

        public JammedGunEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterEventHandler<EventWeaponFire>(OnWeaponFire, HookMode.Post);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.jammed_gun", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowJammed = ctx.State.Toggle(Id, ctx.Target.Slot);
            ctx.PrintToTarget(nowJammed ? "troll.jammed_gun.on.chat" : "troll.jammed_gun.off.chat");
        }

        private HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player is not { IsValid: true } || !_state.IsActive(Id, player.Slot))
                return HookResult.Continue;

            CBasePlayerWeapon? weapon = player.PlayerPawn?.Value?.WeaponServices?.ActiveWeapon?.Value;
            if (weapon is { IsValid: true })
                weapon.Clip1 = 0;

            return HookResult.Continue;
        }
    }
}
