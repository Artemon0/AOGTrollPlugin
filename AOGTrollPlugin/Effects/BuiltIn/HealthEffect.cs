using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn;

public sealed class HealthEffect : ITrollEffect
{
    public string Id => "health";
    public EffectCategory Category => EffectCategory.Combat;

    public IReadOnlyList<TrollEffectVariant> GetVariants() =>
    [
        new("troll.health.heal", ctx => Apply(ctx, 100, "troll.health.heal.chat")),
        new("troll.health.low", ctx => Apply(ctx, 25, "troll.health.low.chat")),
        new("troll.health.high", ctx => Apply(ctx, 200, "troll.health.high.chat"))
    ];

    private static void Apply(TrollEffectContext ctx, int health, string message)
    {
        CCSPlayerPawn? pawn = ctx.Target.PlayerPawn.Value;
        if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive) return;
        pawn.Health = health;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        ctx.PrintToTarget(message);
    }
}
