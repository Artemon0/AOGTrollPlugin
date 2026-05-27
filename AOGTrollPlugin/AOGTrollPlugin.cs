using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace AOGTrollPlugin
{
    public class AOGTrollConfig : BasePluginConfig
    {
        [JsonPropertyName("EnableTrolling")]
        public bool EnableTrolling { get; set; } = true;

        [JsonPropertyName("RequiresFlag")]
        public string RequiresFlag { get; set; } = "@css/troll";

        [JsonPropertyName("TrollInterval")]
        public float TrollInterval { get; set; } = 4.0f;
    }

    public class AOGTrollPlugin : BasePlugin, IPluginConfig<AOGTrollConfig>
    {
        public override string ModuleName => "AOG Troll Plugin";
        public override string ModuleVersion => "1.1.7";
        public override string ModuleAuthor => "AOGames888";

        public AOGTrollConfig Config { get; set; } = new();

        // private readonly Dictionary<int, float> _cursedPlayers = new();

        private readonly Dictionary<int, Vector> _connectionProblemPlayers = new();
        private readonly Random _random = new();

        public HashSet<int> AimbotPlayers = new HashSet<int>();

        public void OnConfigParsed(AOGTrollConfig config)
        {
            var _config = Config;
        }

        public override void Load(bool hotReload)
        {
            Console.WriteLine("[TROLL]!");
        }

        private void StartConnectionProblems(
            CCSPlayerController target,
            bool silent,
            string prefix,
            float duration
        )
        {
            if (target == null || !target.IsValid || !target.PawnIsAlive)
                return;

            if (!silent)
            {
                target.PrintToChat($" {prefix} {Localizer["troll.connection_problems.chat"]}");
            }

            int iterations = 0;

            int maxIterations = (int)(duration / 0.1f);

            CounterStrikeSharp.API.Modules.Timers.Timer? lagTimer = null;

            lagTimer = AddTimer(
                0.1f,
                () =>
                {
                    if (target == null || !target.IsValid || !target.PawnIsAlive)
                    {
                        lagTimer?.Kill();
                        return;
                    }

                    var pawn = target.PlayerPawn?.Value;
                    if (pawn != null && pawn.IsValid)
                    {
                        var currentPos = pawn.AbsOrigin;
                        if (currentPos != null)
                        {
                            float offsetX = (float)(_random.NextDouble() * 80 - 40);
                            float offsetY = (float)(_random.NextDouble() * 80 - 40);
                            float offsetZ = (float)(_random.NextDouble() * 4 - 2);

                            Vector jitterPos = new Vector(
                                currentPos.X + offsetX,
                                currentPos.Y + offsetY,
                                currentPos.Z + offsetZ
                            );

                            pawn.Teleport(jitterPos, null, null);
                        }
                    }

                    iterations++;

                    if (iterations >= maxIterations)
                    {
                        lagTimer?.Kill();
                        if (!silent)
                        {
                            target.PrintToChat(
                                $" {prefix} {Localizer["troll.connection_problems.chat.off"]}"
                            );
                        }
                    }
                },
                CounterStrikeSharp.API.Modules.Timers.TimerFlags.REPEAT
            );
        }

        // private void ToggleAimbot(CCSPlayerController? admin, CCSPlayerController? target)
        // {
        //     if (target == null || !target.IsValid || !target.PawnIsAlive)
        // }

        [ConsoleCommand("css_atroll", "Troll a player")]
        // [RequiresPermissions("@css/troll")]
        public void TrollPlayer(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid)
                return;

            string prefix = Localizer["troll.chat.prefix"];

            // menu
            CenterHtmlMenu trollMenu = new(Localizer["player.select"], this);
            var players = Utilities
                .GetPlayers()
                .Where(p => p.IsValid && !p.IsBot && p.Team != CsTeam.Spectator)
                .ToList();

            foreach (var i in players)
            {
                var target = i;

                trollMenu.AddMenuOption(
                    target.PlayerName,
                    (admin, option) =>
                    {
                        CenterHtmlMenu Pmenu = new(
                            $"{Localizer["player.troll"]}: {target.PlayerName}",
                            this
                        );

                        Pmenu.AddMenuOption(
                            Localizer["troll.tpskybox"],
                            (adminCtrl, opt) =>
                            {
                                var pawn = target.PlayerPawn?.Value;
                                if (pawn == null || !pawn.IsValid)
                                    return;

                                var currentPos = pawn.AbsOrigin;
                                if (currentPos != null)
                                {
                                    Vector newPos = new Vector(
                                        currentPos.X,
                                        currentPos.Y,
                                        currentPos.Z + 1000.0f
                                    );
                                    pawn.Teleport(newPos, null, null);
                                }

                                target.PrintToChat(prefix + Localizer["troll.tpskybox.chat"]);
                            }
                        );

                        Pmenu.AddMenuOption(
                            Localizer["troll.stripweapons"],
                            (adminCtrl, opt) =>
                            {
                                var pawn = target.PlayerPawn?.Value;
                                if (pawn?.WeaponServices?.MyWeapons == null)
                                    return;

                                foreach (var weapon in pawn.WeaponServices.MyWeapons)
                                {
                                    if (weapon?.Value != null && weapon.Value.IsValid)
                                    {
                                        weapon.Value.Remove();
                                        target.PrintToChat(
                                            prefix + Localizer["troll.stripweapons.chat"]
                                        );
                                    }
                                }
                            }
                        );

                        Pmenu.AddMenuOption(
                            Localizer["troll.kill"],
                            (adminCtrl, opt) =>
                            {
                                var pawn = target.PlayerPawn?.Value;

                                if (pawn != null && pawn.IsValid && target.PawnIsAlive)
                                {
                                    pawn.Health = 0;
                                    target.PrintToChat(prefix + Localizer["troll.kill.chat"]);
                                }
                            }
                        );

                        Pmenu.AddMenuOption(
                            Localizer["troll.connection_problems"],
                            (adminCtrl, opt) =>
                            {
                                CenterHtmlMenu subMenu = new(
                                    Localizer["troll.connection_problems"],
                                    this
                                );
                                subMenu.AddMenuOption(
                                    Localizer["troll.connection_problems.with_chat"],
                                    (a, o) => StartConnectionProblems(target, false, prefix, 5)
                                );
                                subMenu.AddMenuOption(
                                    Localizer["troll.connection_problems.silent"],
                                    (a, o) => StartConnectionProblems(target, true, prefix, 5)
                                );
                                MenuManager.OpenCenterHtmlMenu(this, adminCtrl, subMenu);
                            }
                        );

                        Pmenu.AddMenuOption(
                            Localizer["troll.noclip"],
                            (adminCtrl, opt) =>
                            {
                                var pawn = target.Pawn?.Value;
                                if (pawn != null && pawn.IsValid)
                                {
                                    if (pawn.MoveType == MoveType_t.MOVETYPE_NOCLIP)
                                    {
                                        pawn.MoveType = MoveType_t.MOVETYPE_WALK;
                                        target.PrintToChat(
                                            $" {prefix} {Localizer["troll.noclipon.chat"]} "
                                        );
                                    }
                                    else
                                    {
                                        pawn.MoveType = MoveType_t.MOVETYPE_NOCLIP;
                                        target.PrintToChat(
                                            $" {prefix} {Localizer["troll.noclipoff.chat"]} "
                                        );
                                    }
                                }
                            }
                        );

                        // Pmenu.AddMenuOption(
                        //     Localizer["troll.aimbot"],
                        //     (adminCtrl, opt) =>
                        //     {
                        //         CenterHtmlMenu subMenu = new(
                        //             Localizer["troll.aimbot.silent"],
                        //             this
                        //         );

                        //         subMenu.AddMenuOption(
                        //             Localizer["troll.aimbot.silent"],
                        //             (a, o) => { }
                        //         );

                        //         if (target == null || !target.IsValid)
                        //         {
                        //             adminCtrl.PrintToChat(
                        //                 " \x02[Troll] \x01Игрок не найден или уже вышел."
                        //             );
                        //             return;
                        //         }

                        //         int targetSlot = target.Slot;

                        //         if (AimbotPlayers.Contains(targetSlot))
                        //         {
                        //             AimbotPlayers.Remove(targetSlot);

                        //             adminCtrl.PrintToChat(
                        //                 $" \x02[Troll] \x01Аимбот для \x06{target.PlayerName} \x02ВЫКЛЮЧЕН\x01."
                        //             );
                        //             target.PrintToChat(
                        //                 " \x02[Troll] \x01Твои читерские способности пропали :("
                        //             );
                        //         }
                        //         else
                        //         {
                        //             AimbotPlayers.Add(targetSlot);

                        //             adminCtrl.PrintToChat(
                        //                 $" \x02[Troll] \x01Аимбот для \x06{target.PlayerName} \x06ВКЛЮЧЕН\x01."
                        //             );
                        //             target.PrintToChat(
                        //                 " \x02[Troll] \x01У тебя активирован режим терминатора! Стреляй куда угодно."
                        //             );
                        //         }
                        //     }
                        // );

                        MenuManager.OpenCenterHtmlMenu(this, admin, Pmenu);
                    }
                );
            }
            MenuManager.OpenCenterHtmlMenu(this, player, trollMenu);
        }
    }
}

/* localization:
localization      default(en)
--------------------------------------
player.select - select player to troll
player.troll - troll a {player.PlayerName}
troll.tpskybox - teeleport a {Player} to skybox
troll.chat.prefix - " \u0002[Troll]\u0001 "
troll.tpskybox.chat - you has been teleported to skybox
troll.stripweapons - strip weapons
troll.stripweapons.chat - Your weapons has been stolen
troll.kill - kill player
troll.kill.chat -  you have been killed
troll.connection_problems - Make connection problems
troll.connection_problems.with_chat - Troll with chat
troll.connection_problems.chat.off - Connection repaired!
troll.connection_problems.silent - S1lent troll
troll.noclip - N0clip
troll.noclipon.chat - Admin turned on noclip for you!
troll.noclipoff.chat - Admin turned off your noclip
troll.aimbot - Aimbot
troll.aimbot.issilent - Is silent

*/
