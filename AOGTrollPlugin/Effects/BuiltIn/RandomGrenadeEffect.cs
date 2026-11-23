namespace AOGTrollPlugin.Effects.BuiltIn;

public sealed class RandomGrenadeEffect : ITrollEffect
{
    private static readonly string[] Grenades =
    [
        "weapon_hegrenade",
        "weapon_flashbang",
        "weapon_smokegrenade",
        "weapon_decoy"
    ];

    public string Id => "random_grenade";
    public EffectCategory Category => EffectCategory.Combat;

    public IReadOnlyList<TrollEffectVariant> GetVariants() => [new("troll.random_grenade", Run)];

    private static void Run(TrollEffectContext ctx)
    {
        if (!ctx.Target.PawnIsAlive)
        {
            ctx.PrintToAdmin("troll.random_grenade.unavailable");
            return;
        }

        string grenade = Grenades[Random.Shared.Next(Grenades.Length)];
        if (ctx.Target.GiveNamedItem(grenade) == 0)
        {
            ctx.PrintToAdmin("troll.random_grenade.failed");
            return;
        }

        ctx.PrintToTarget("troll.random_grenade.chat");
    }
}
