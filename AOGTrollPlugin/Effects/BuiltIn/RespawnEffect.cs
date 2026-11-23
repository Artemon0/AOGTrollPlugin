using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn;

public sealed class RespawnEffect : ITrollEffect
{
    public string Id => "respawn";
    public EffectCategory Category => EffectCategory.Utility;
    public bool IncludeInRandomSelection => false;

    public IReadOnlyList<TrollEffectVariant> GetVariants() => [new("troll.respawn", Run)];

    private static void Run(TrollEffectContext ctx)
    {
        if (ctx.Target.PawnIsAlive || ctx.Target.Team is not (CsTeam.Terrorist or CsTeam.CounterTerrorist))
        {
            ctx.PrintToAdmin("troll.respawn.unavailable");
            return;
        }

        ctx.Target.Respawn();
        ctx.PrintToTarget("troll.respawn.chat");
    }
}
