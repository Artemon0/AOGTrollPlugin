using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class DisarmToKnifeEffect : ITrollEffect
    {
        public string Id => "disarm_to_knife";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.disarm_to_knife", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn?.WeaponServices?.MyWeapons == null)
                return;

            bool removedAny = false;
            foreach (CHandle<CBasePlayerWeapon> weapon in pawn.WeaponServices.MyWeapons)
            {
                if (weapon?.Value is not { IsValid: true } w)
                    continue;

                if (w.DesignerName is "weapon_knife" or "weapon_c4")
                    continue;

                w.Remove();
                removedAny = true;
            }

            if (removedAny)
                ctx.PrintToTarget("troll.disarm_to_knife.chat");
        }
    }
}
