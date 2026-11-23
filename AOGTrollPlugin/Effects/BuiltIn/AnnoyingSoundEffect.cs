using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class AnnoyingSoundEffect(AOGTrollConfig config) : ITrollEffect
    {
        public string Id => "annoying_sound";
        public EffectCategory Category => EffectCategory.Visual;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new("troll.annoying_sound", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn.Value;
            if (pawn is not { IsValid: true })
                return;

            RecipientFilter filter = new(ctx.Target);

            pawn.EmitSound(config.SoundEffectEvent, filter, 1.0f, 100.0f);

            ctx.PrintToTarget("troll.annoying_sound.chat");
        }
    }
}
