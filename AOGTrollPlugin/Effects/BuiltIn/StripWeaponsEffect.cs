using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class StripWeaponsEffect : ITrollEffect
    {
        public string Id => "stripweapons";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.stripweapons", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn?.WeaponServices?.MyWeapons == null)
                return;

            bool removedAny = false;
            foreach (CHandle<CBasePlayerWeapon> weapon in pawn.WeaponServices.MyWeapons)
            {
                if (weapon?.Value is not { IsValid: true })
                    continue;

                weapon.Value.Remove();
                removedAny = true;
            }

            if (removedAny)
                ctx.PrintToTarget("troll.stripweapons.chat");
        }
    }
}
