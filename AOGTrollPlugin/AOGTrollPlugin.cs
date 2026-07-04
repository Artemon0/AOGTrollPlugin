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
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace AOGTrollPlugin
{
    public class AOGTrollConfig : BasePluginConfig
    {
        [JsonPropertyName("EnableTrolling")] public bool EnableTrolling { get; set; } = true;

        [JsonPropertyName("RequiresFlag")] public string RequiresFlag { get; set; } = "@css/troll";
    }

    public class AOGTrollPlugin : BasePlugin, IPluginConfig<AOGTrollConfig>
    {
        public override string ModuleName => "AOG Troll Plugin";
        public override string ModuleVersion => "1.1.8";
        public override string ModuleAuthor => "AOGames888";

        public AOGTrollConfig Config { get; set; } = new();

        // private readonly Dictionary<int, float> _cursedPlayers = new();

        // private readonly Dictionary<int, Vector> _connectionProblemPlayers = new();
        private readonly Random _random = new();

        private HashSet<int> _aimbotPlayers = new HashSet<int>();

        private readonly string[] _allowedWeapons =
        [
            // pistols
            "weapon_deagle", // Desert Eagle[reference:0][reference:1]
            "weapon_elite", // Dual Berettas[reference:2][reference:3]
            "weapon_fiveseven", // Five-SeveN[reference:4][reference:5]
            "weapon_glock", // Glock-18[reference:6][reference:7]
            "weapon_hkp2000", // P2000[reference:8][reference:9]
            "weapon_p250", // P250[reference:10][reference:11]
            "weapon_usp_silencer", // USP-S[reference:12][reference:13]
            "weapon_tec9", // Tec-9[reference:14][reference:15]
            "weapon_cz75a", // CZ75-Auto[reference:16][reference:17]
            "weapon_revolver", // R8 Revolver[reference:18][reference:19]

            // smg
            "weapon_mac10", // MAC-10[reference:20][reference:21]
            "weapon_mp9", // MP9[reference:22][reference:23]
            "weapon_mp7", // MP7[reference:24][reference:25]
            "weapon_mp5sd", // MP5-SD[reference:26][reference:27]
            "weapon_p90", // P90[reference:28][reference:29]
            "weapon_bizon", // PP-Bizon[reference:30][reference:31]
            "weapon_ump45", // UMP-45[reference:32][reference:33]

            // rifles
            "weapon_ak47", // AK-47[reference:34][reference:35]
            "weapon_galilar", // Galil AR[reference:36][reference:37]
            "weapon_famas", // FAMAS[reference:38][reference:39]
            "weapon_m4a1", // M4A4[reference:40][reference:41]
            "weapon_m4a1_silencer", // M4A1-S[reference:42][reference:43]
            "weapon_aug", // AUG[reference:44][reference:45]
            "weapon_sg556", // SG 553[reference:46][reference:47]

            // sniper rifles
            "weapon_awp", // AWP[reference:48][reference:49]
            "weapon_ssg08", // SSG 08[reference:50][reference:51]
            "weapon_scar20", // SCAR-20[reference:52][reference:53]
            "weapon_g3sg1", // G3SG1[reference:54][reference:55]

            // shotguns
            "weapon_nova", // Nova[reference:56][reference:57]
            "weapon_xm1014", // XM1014[reference:58][reference:59]
            "weapon_mag7", // MAG-7[reference:60][reference:61]
            "weapon_sawedoff", // Sawed-Off[reference:62][reference:63]

            // machine guns
            "weapon_m249", // M249[reference:64][reference:65]
            "weapon_negev" // Negev[reference:66][reference:67]
        ];

        public void OnConfigParsed(AOGTrollConfig config)
        {
            var _config = Config;
        }

        public override void Load(bool hotReload)
        {
            if (!Config.EnableTrolling) return;

            Console.WriteLine("[TROLL] Plugin loaded!");

            // RegisterListener<Listeners.OnTick>(OnTickHandler);
            RegisterEventHandler<EventWeaponFire>(Aimbot, HookMode.Pre);
        }

        public override void Unload(bool hotReload) { }

        /*private void OnTickHandler()
        {
            if (_aimbotPlayers.Count == 0)
                return;

            foreach (var slot in _aimbotPlayers.ToList())
            {
                var player = Utilities.GetPlayerFromSlot(slot);
                if (player == null || !player.IsValid || !player.PawnIsAlive)
                    continue;

                var pawn = player.PlayerPawn?.Value;
                if (pawn == null || !pawn.IsValid)
                    continue;

                var target = FindClosestEnemy(player);
                if (target == null)
                    continue;

                var targetPawn = target.PlayerPawn?.Value;
                if (targetPawn == null || !targetPawn.IsValid)
                    continue;

                var sourcePos = pawn.AbsOrigin;
                if (sourcePos == null)
                    continue;

                Vector eyePos = new Vector(
                    sourcePos.X + pawn.ViewOffset.X,
                    sourcePos.Y + pawn.ViewOffset.Y,
                    sourcePos.Z + pawn.ViewOffset.Z
                );

                var targetPos = targetPawn.AbsOrigin;
                if (targetPos == null)
                    continue;

                Vector targetHeadPos = new Vector(targetPos.X, targetPos.Y, targetPos.Z + 55.0f);

                float deltaX = targetHeadPos.X - eyePos.X;
                float deltaY = targetHeadPos.Y - eyePos.Y;
                float deltaZ = targetHeadPos.Z - eyePos.Z;

                float hypotenuse = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                float yaw = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI);
                float pitch = (float)(-Math.Atan2(deltaZ, hypotenuse) * 180.0 / Math.PI);

                pawn.Teleport(null, new QAngle(pitch, yaw, 0.0f), null);
            }
        }*/

        public HookResult Aimbot(EventWeaponFire @event, GameEventInfo info)
        {
            if (_aimbotPlayers.Count == 0) return HookResult.Continue;

            if (!_allowedWeapons.Contains(@event.Weapon)) return HookResult.Continue;

            foreach (int slot in _aimbotPlayers.ToList())
            {
                CCSPlayerController? player = Utilities.GetPlayerFromSlot(slot);
                if (player == null || !player.IsValid || !player.PawnIsAlive)
                    continue;

                CCSPlayerPawn? pawn = player.PlayerPawn?.Value;
                if (pawn == null || !pawn.IsValid)
                    continue;

                CCSPlayerController? target = FindClosestEnemy(player);
                if (target == null)
                    continue;

                CCSPlayerPawn? targetPawn = target.PlayerPawn?.Value;
                if (targetPawn == null || !targetPawn.IsValid)
                    continue;

                Vector? sourcePos = pawn.AbsOrigin;
                if (sourcePos == null)
                    continue;

                Vector eyePos = new Vector(
                    sourcePos.X + pawn.ViewOffset.X,
                    sourcePos.Y + pawn.ViewOffset.Y,
                    sourcePos.Z + pawn.ViewOffset.Z
                );

                Vector? targetPos = targetPawn.AbsOrigin;
                if (targetPos == null)
                    continue;

                Vector targetHeadPos = new Vector(targetPos.X, targetPos.Y, targetPos.Z + 55.0f);

                float deltaX = targetHeadPos.X - eyePos.X;
                float deltaY = targetHeadPos.Y - eyePos.Y;
                float deltaZ = targetHeadPos.Z - eyePos.Z;

                float hypotenuse = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                float yaw = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI);
                float pitch = (float)(-Math.Atan2(deltaZ, hypotenuse) * 180.0 / Math.PI);

                QAngle oldAngles = pawn.EyeAngles;


                pawn.Teleport(null, new QAngle(pitch, yaw, 0.0f));

                AddTimer(0.01f, () =>
                {
                    if (player.IsValid && pawn.IsValid && player.PawnIsAlive)
                        pawn.Teleport(null, oldAngles);
                });
            }

            return HookResult.Continue;
        }

        private CCSPlayerController? FindClosestEnemy(CCSPlayerController player)
        {
            CCSPlayerPawn? pawn = player.PlayerPawn?.Value;

            if (pawn == null || !pawn.IsValid || pawn.AbsOrigin == null)
                return null;

            CCSPlayerController? closestEnemy = null;
            float minDistance = float.MaxValue;

            var enemies = Utilities
                .GetPlayers()
                .Where(p =>
                    p.IsValid && p.PawnIsAlive && p.TeamNum != player.TeamNum && p.TeamNum > 1
                );

            foreach (var enemy in enemies)
            {
                var enemyPawn = enemy.PlayerPawn?.Value;
                if (enemyPawn == null || !enemyPawn.IsValid || enemyPawn.AbsOrigin == null)
                    continue;

                float dist = GetDistance(pawn.AbsOrigin, enemyPawn.AbsOrigin);
                if (!(dist < minDistance)) continue;
                minDistance = dist;
                closestEnemy = enemy;
            }

            return closestEnemy;
        }

        private float GetDistance(Vector v1, Vector v2)
        {
            float dX = v1.X - v2.X;
            float dY = v1.Y - v2.Y;
            float dZ = v1.Z - v2.Z;
            return (float)Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        private void StartConnectionProblems(
            CCSPlayerController? target,
            bool silent,
            string? prefix,
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

            Timer? lagTimer = null;

            lagTimer = AddTimer(
                0.1f,
                () =>
                {
                    if (!target.IsValid || !target.PawnIsAlive)
                    {
                        lagTimer?.Kill();
                        return;
                    }

                    CCSPlayerPawn? pawn = target.PlayerPawn.Value;
                    if (pawn != null && pawn.IsValid)
                    {
                        Vector? currentPos = pawn.AbsOrigin;
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
                TimerFlags.REPEAT
            );
        }

        private void ToggleAimbot(
            CCSPlayerController? admin,
            CCSPlayerController? target,
            bool silent
        )
        {
            if (target == null || !target.IsValid || !target.PawnIsAlive) return;
            if (admin == null || !admin.IsValid) return;

            string prefix = Localizer["troll.chat.prefix"];
            int targetSlot = target.Slot;
            if (_aimbotPlayers.Remove(targetSlot))
            {
                // _aimbotPlayers.Remove(targetSlot);
                admin.PrintToChat($" {prefix} {Localizer["disabled"]}");
                if (!silent)
                {
                    target.PrintToChat($" {prefix} {Localizer["troll.aimbot.disabled"]}");
                }
            }
            else
            {
                _aimbotPlayers.Add(targetSlot);
                if (!silent)
                    target.PrintToChat($" {prefix} {Localizer["troll.aimbot.disabled"]}");
                admin.PrintToChat($" {prefix} {Localizer["troll.aimbot.admin_enabled"]}"); // dont forget to localize!
            }
        }


        [ConsoleCommand("css_atroll", "Troll a player")]
        [RequiresPermissions("@css/troll")]
        public void TrollPlayer(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid)
                return;

            string prefix = Localizer["troll.chat.prefix"];

            // menu
            CenterHtmlMenu trollMenu = new(Localizer["player.select"], this);
            var players = Utilities
                .GetPlayers()
                .Where(p => p is { IsValid: true, IsBot: false } && p.Team != CsTeam.Spectator)
                .ToList();

            foreach (CCSPlayerController i in players)
            {
                CCSPlayerController target = i;

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
                                CCSPlayerPawn? pawn = target.PlayerPawn?.Value;
                                if (pawn == null || !pawn.IsValid)
                                    return;

                                Vector? currentPos = pawn.AbsOrigin;
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
                                CCSPlayerPawn? pawn = target.PlayerPawn?.Value;
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
                                CCSPlayerPawn? pawn = target.PlayerPawn?.Value;

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
                                CBasePlayerPawn? pawn = target.Pawn?.Value;
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

                        Pmenu.AddMenuOption(
                            Localizer["troll.aimbot"],
                            (adminCtrl, opt) =>
                            {
                                CenterHtmlMenu subMenu = new(
                                    Localizer["troll.aimbot.issilent"],
                                    this
                                );

                                subMenu.AddMenuOption(
                                    Localizer["troll.aimbot.silent"],
                                    (a, o) => { ToggleAimbot(a, target, true); }
                                );

                                subMenu.AddMenuOption(
                                    Localizer["troll.aimbot"],
                                    (a, o) => { ToggleAimbot(a, target, false); }
                                );
                            }
                        );

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
troll.aimbot.silent - Aimbot (s1lent)

*/