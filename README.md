# AOG Troll Plugin

## Plugin Overview
**AOG Troll Plugin** is an administrative plugin for the CounterStrikeSharp (CS2) platform, designed for entertainment and trolling players on the server via a convenient center HTML menu. The plugin allows administrators to apply various effects to selected players, ranging from simulating network connection issues to teleporting them into the skybox and stripping their weapons.

* **Version:** 1.1.7
* **Author:** AOGames888
* **Platform:** CounterStrikeSharp (CS2)

---

## Installation
1. Download the plugin files.
2. Place the plugin folder into the server directory: `addons/counterstrikesharp/plugins/`.
3. Restart the server or change the map to automatically generate the configuration file.

---

## Configuration
After the first launch, a `config.json` file will be created in ```addons/counterstrikesharp/configs/plugins/AOGTrollPlugin```. You can configure the following parameters:

| Parameter | Data Type | Default Value | Description |
| :--- | :--- | :--- | :--- |
| `EnableTrolling` | Boolean | `true` | Enables or disables the core functionality of the plugin. |
| `RequiresFlag` | String | `"@css/troll"` | The admin permission flag required to access the menu. |
| `TrollInterval` | Float | `4.0f` | Internal interval used for trolling timers. |
```
TrollInterval is now unused, you can not configure it
```

---

## Commands and Usage

### Main Command
* **`css_atroll`** — Opens the player selection menu in the center of the screen. This command is restricted to administrators with the required permissions.

---

## Troll Menu Features

Once the command is executed and a target player is selected, the administrator can access the following features:

* **Teleport to Skybox** — Instantly teleports the player 1000 units up along the Z-axis.
* **Strip Weapons** — Clears the player's inventory entirely, removing all current weapons and grenades.
* **Kill Player** — Instantly sets the target player's health to 0.
* **Connection Problems** — Simulates severe network lag. For a duration of 5 seconds, the plugin teleports the player to a random nearby position every 0.1 seconds, creating a heavy desynchronization (jitter) effect. This feature can be executed in two modes:
  * With a public chat notification.
  * Silent mode.
* **Noclip** — Toggles the ability to fly and pass through walls for the selected player.

> **Note:** The source code contains a commented-out placeholder for an "Aimbot" feature (a joke feature intended to give a player automated targeting capabilities). This functionality is currently disabled and planned for future updates.

---

## Localization (Translation Files)
The plugin supports the CounterStrikeSharp localization system. Below is the list of language keys used within the translation files:

```txt
player.select - Select player to troll
player.troll - Troll a {player.PlayerName}
troll.tpskybox - Teleport a {Player} to skybox
troll.chat.prefix - " [Troll] "
troll.tpskybox.chat - You have been teleported to skybox
troll.stripweapons - Strip weapons
troll.stripweapons.chat - Your weapons have been stolen
troll.kill - Kill player
troll.kill.chat - You have been killed
troll.connection_problems - Make connection problems
troll.connection_problems.with_chat - Troll with chat
troll.connection_problems.chat.off - Connection repaired!
troll.connection_problems.silent - Silent troll
troll.noclip - Noclip
troll.noclipon.chat - Admin turned on noclip for you!
troll.noclipoff.chat - Admin turned off your noclip
troll.aimbot - Aimbot
troll.aimbot.issilent - Is silent