namespace AOGTrollPlugin.Effects.BuiltIn;

public sealed class RandomEffect(Action<TrollEffectContext> run) : ITrollEffect
{
    public string Id => "random";
    public EffectCategory Category => EffectCategory.Utility;

    public IReadOnlyList<TrollEffectVariant> GetVariants() =>
    [
        new TrollEffectVariant("troll.random", run)
    ];
}
