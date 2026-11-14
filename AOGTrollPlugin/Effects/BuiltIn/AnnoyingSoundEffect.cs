using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Plays a sound that only the target can hear, using EmitSound with a
    /// RecipientFilter containing just them. The sound event name comes from
    /// config (AOGTrollConfig.SoundEffectEvent) rather than being hardcoded,
    /// since CS2 plays sounds by soundevent name (from the game's .vsndevts
    /// files) and I can't verify from here which event names exist on any
    /// given server build - if the shipped default doesn't play anything,
    /// swap in any event name pulled from Source 2 Viewer.
    /// </summary>
    public sealed class AnnoyingSoundEffect : ITrollEffect
    {
        private readonly AOGTrollConfig _config;

        public string Id => "annoying_sound";
        public EffectCategory Category => EffectCategory.Visual;

        public AnnoyingSoundEffect(AOGTrollConfig config)
        {
            _config = config;
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.annoying_sound", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn.Value;
            if (pawn is not { IsValid: true })
                return;

            RecipientFilter filter = new(ctx.Target);

            // The two float params after the filter aren't confirmed from
            // here - going with (volume, pitch) since that matches the
            // equivalent SourceMod native, but double check against
            // CBaseEntity.EmitSound's actual doc comment in your installed
            // CounterStrikeSharp.API if the sound plays oddly.
            pawn.EmitSound(_config.SoundEffectEvent, filter, 1.0f, 100.0f);

            ctx.PrintToTarget("troll.annoying_sound.chat");
        }
    }
}
