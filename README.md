# AOGTrollPlugin

Administrative effects plugin for Counter-Strike 2 and CounterStrikeSharp. [Русский](README_RU.md) · [Documentation](https://artemon0.github.io/AOGTrollPlugin/)

## Installation

1. Install [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) 1.0.374 or newer, [PlayerSettingsCS2](https://github.com/NickFox007/PlayerSettingsCS2) 0.9.4 or newer, and [MenuManagerCS2](https://github.com/karola3vax/MenuManagerCS2/releases/latest).
2. Make sure MenuManager's `MenuManagerApi.dll` is installed in `addons/counterstrikesharp/shared/MenuManagerApi/`.
3. Download the [latest release](https://github.com/Artemon0/AOGTrollPlugin/releases/latest) and extract its `AOGTrollPlugin` folder into `addons/counterstrikesharp/plugins/`.
4. Restart the server or change the map to generate the configuration.

## Configuration

Edit `addons/counterstrikesharp/configs/plugins/AOGTrollPlugin/config.json`:

- `EnableTrolling`: enables the plugin.
- `RequiresFlag`: required administrator flag; default `@css/troll`.
- `SoundEffectEvent`: sound event for the annoying sound effect.
- `DiscordWebhookUrls`: optional array of Discord webhook URLs; keep them private.
- `Effects`: enable or disable individual effects by ID.

Use `css_atroll` or `!atroll` to open the menu. Administrators need both `@css/troll` and the configured `RequiresFlag` (if different). See the [documentation](https://artemon0.github.io/AOGTrollPlugin/) for effects, configuration and development details.

## Build

Install the .NET 10 SDK and run `dotnet build AOGTrollPlugin/AOGTrollPlugin.csproj -c Release`. The bundled MenuManager API contract comes from [MenuManagerCS2](https://github.com/karola3vax/MenuManagerCS2), commit `0c38f7c8f5ebf05d14b18e9e974cf38af9c4b071`; the server must use the shared API distributed with MenuManager.
