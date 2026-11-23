---
layout: default
title: Installation
---

# Installation

## Requirements

Install CounterStrikeSharp 1.0.374 or newer, [PlayerSettingsCS2 0.9.4 or newer](https://github.com/NickFox007/PlayerSettingsCS2) and [MenuManagerCS2 by karola3vax](https://github.com/karola3vax/MenuManagerCS2/releases/latest). Install their release archives as instructed by each project; MenuManager's shared `MenuManagerApi.dll` must be in `addons/counterstrikesharp/shared/MenuManagerApi/`. Install the AOGTrollPlugin release under `addons/counterstrikesharp/plugins/AOGTrollPlugin/`.

The plugin uses `menu:nfcore` and honors the player's menu selection (`!menus`). If the shared API is present but the menu capability is unavailable, it uses CounterStrikeSharp's center menu. The shared API is a required runtime dependency even for this fallback.

## Install the plugin

1. Download the [latest release](https://github.com/Artemon0/AOGTrollPlugin/releases/latest).
2. Extract the plugin folder into `addons/counterstrikesharp/plugins/`.
3. Restart the server or change the map.
4. Open [Configuration](configuration.html) and grant the administrator permission.

## Verify installation

Run `css_atroll` as an authorized administrator in game. If the menu is unavailable, confirm the dependencies and examine the server console.
