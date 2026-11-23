using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn;

public sealed class AmmoEffect : ITrollEffect
{
    public string Id => "ammo";
    public EffectCategory Category => EffectCategory.Combat;

    public IReadOnlyList<TrollEffectVariant> GetVariants() =>
    [
        new("troll.ammo.refill", ctx => Apply(ctx, refill: true)),
        new("troll.ammo.empty", ctx => Apply(ctx, refill: false))
    ];

    private static void Apply(TrollEffectContext ctx, bool refill)
    {
        CBasePlayerWeapon? weapon = ctx.Target.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;
        if (!ctx.Target.PawnIsAlive || weapon is not { IsValid: true, Clip1: >= 0 } ||
            weapon.As<CCSWeaponBase>().VData is not { MaxClip1: > 0, PrimaryReserveAmmoMax: > 0 } data)
        {
            ctx.PrintToAdmin("troll.ammo.unavailable");
            return;
        }

        weapon.Clip1 = refill ? data.MaxClip1 : 0;
        weapon.ReserveAmmo[0] = refill ? data.PrimaryReserveAmmoMax : 0;
        Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_iClip1");
        Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_pReserveAmmo");
        ctx.PrintToTarget(refill ? "troll.ammo.refill.chat" : "troll.ammo.empty.chat");
    }
}
