using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class RandomWeaponEffect : ITrollEffect
    {
        private static readonly string[] Pool =
        [
            "weapon_ak47",
            "weapon_m4a1",
            "weapon_awp",
            "weapon_deagle",
            "weapon_mac10",
            "weapon_nova",
            "weapon_negev",
            "weapon_taser"
        ];

        public string Id => "random_weapon";
        public EffectCategory Category => EffectCategory.Combat;

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.random_weapon", Run)
        ];

        private static void Run(TrollEffectContext ctx)
        {
            CCSPlayerPawn? pawn = ctx.Target.PlayerPawn?.Value;
            if (pawn is not { IsValid: true } || !ctx.Target.PawnIsAlive)
                return;

            string weapon = Pool[Random.Shared.Next(Pool.Length)];
            ctx.Target.GiveNamedItem(weapon);

            ctx.PrintToTarget("troll.random_weapon.chat");
        }
    }
}
