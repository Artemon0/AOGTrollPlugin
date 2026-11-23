using System;
using System.Collections.Generic;

namespace AOGTrollPlugin.Effects
{
    public sealed record TrollEffectVariant(string LabelLocalizationKey, Action<TrollEffectContext> Run);

    public interface ITrollEffect
    {
        string Id { get; }

        EffectCategory Category { get; }

        bool IncludeInRandomSelection => true;

        IReadOnlyList<TrollEffectVariant> GetVariants();
    }
}
