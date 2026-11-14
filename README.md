# AOG Troll Plugin

Administrative "troll" plugin for CounterStrikeSharp (CS2). Admins pick a
player from an in-game menu and apply an effect to them — from teleporting
them into the skybox to giving them a wobbly drunk camera for a few seconds.

- **Version:** 2.4.0
- **Author:** AOGames888
- **Platform:** CounterStrikeSharp (CS2)

## Installation and build

Install CounterStrikeSharp 1.0.374 or newer, [PlayerSettingsCS2 0.9.4 or newer](https://github.com/NickFox007/PlayerSettingsCS2) and [MenuManagerCS2 by karola3vax](https://github.com/karola3vax/MenuManagerCS2/releases/latest). Install their release archives as instructed by each project; MenuManager's shared `MenuManagerApi.dll` must be in `addons/counterstrikesharp/shared/MenuManagerApi/`. Install the AOGTrollPlugin release under `addons/counterstrikesharp/plugins/AOGTrollPlugin/`.

The plugin uses `menu:nfcore` and honors the player's menu selection (`!menus`). If the shared API is present but the menu capability is unavailable, it uses CounterStrikeSharp's center menu. The shared API is a required runtime dependency even for this fallback.

Build with `dotnet build AOGTrollPlugin/AOGTrollPlugin.csproj -c Release` using .NET 10. The API contract is included in `vendor/MenuManagerApi` for reproducible builds; do not install its build output over the shared API shipped with MenuManager. GitHub Actions builds branch pushes and pull requests. To publish a release, set the project version in `AOGTrollPlugin.csproj`, commit it, and push a matching tag (for example `git tag v2.4.0` followed by `git push origin v2.4.0`).

The Health effect provides 25, 100, and 200 HP options.

---

See the [changelog](CHANGELOG.md) for the release history.

---

## Anti-Cheat effects: what they can and can't do

Both `blind_enemies` and `spectator_lock` gate access to `css_atroll` the
same way every other effect does — through `RequiresFlag` — there's no
separate, stricter permission for them. If your admins who get `@css/troll`
shouldn't also be able to punish cheaters this way, split them onto a
narrower flag and gate a copy of the command, or ask and I'll wire that up.

`blind_enemies` needs a CounterStrikeSharp build that has the `CheckTransmit`
listener (`Listeners.CheckTransmit` / `CCheckTransmitInfoList`) — this landed
in the mainline API a while back, but if your server is on an older pinned
build the plugin will fail to load with a missing-type error and you'll need
to update `CounterStrikeSharp.API` in `AOGTrollPlugin.csproj`.

---

## Installation

1. Build the project (or grab the compiled DLL) and drop the plugin folder
   into `addons/counterstrikesharp/plugins/`.
2. Restart the server or change the map to auto-generate `config.json`.

---

## Configuration

Generated at `addons/counterstrikesharp/configs/plugins/AOGTrollPlugin/config.json`
on first launch:

| Parameter        | Type              | Default        | Description                                                        |
| ----------------- | ----------------- | -------------- | -------------------------------------------------------------------- |
| `EnableTrolling`  | Boolean           | `true`         | Master on/off switch for the whole plugin.                          |
| `RequiresFlag`    | String            | `"@css/troll"` | Admin flag required to open `css_atroll` (checked at runtime now).  |
| `SoundEffectEvent`| String            | `"UI.WeaponPurchase"` | Soundevent name played by `annoying_sound` — swap it if the default doesn't play on your build. |
| `Effects`         | Object (bool map) | all `true`     | Per-effect toggle, keyed by effect id — see table below.            |

Example `Effects` block (trimmed):

```json
"Effects": {
  "skybox": true,
  "stripweapons": true,
  "kill": true,
  "one_hp": true,
  "slap": true,
  "disarm_to_knife": true,
  "ignite": true,
  "connection_problems": true,
  "noclip": true,
  "gravity": true,
  "freeze": true,
  "drunk_vision": true,
  "invisible": true,
  "earthquake": false,
  "spin": true,
  "flash": true,
  "random_teleport": true,
  "mute": true,
  "blind_enemies": true,
  "spectator_lock": true,
  "decoy_bot": true,
  "block_defuse": true,
  "stream_blind": true,
  "scale": true,
  "godmode": true,
  "swap_positions": true,
  "annoying_sound": true,
  "stuck_key": true,
  "money": true,
  "upside_down": true,
  "hazy_vision": true,
  "launch": true,
  "sniper_vision": true,
  "random_weapon": true,
  "tint": true,
  "armor": true,
  "jammed_gun": true,
  "teleport_spam": true
}
```

Set any of these to `false` to remove that effect from the menu entirely —
no restart-breaking required, just a config reload / map change.

---

## Commands

- **`css_atroll`** — Opens the player selection menu. Restricted to admins
  holding the flag configured in `RequiresFlag` (`@css/troll` by default).

Menu flow: **pick a player → pick a category → pick an effect** (effects
with more than one mode, like Connection Problems, open one extra sub-menu).

---

## Built-in effects

| Category   | Effect             | Id                    | Description                                                                 |
| ---------- | ------------------- | ---------------------- | ----------------------------------------------------------------------------- |
| Movement   | Teleport to Skybox  | `skybox`               | Teleports the player 1000 units straight up.                                 |
| Movement   | Slap                | `slap`                 | Classic random-direction knockback impulse.                                  |
| Movement   | Noclip              | `noclip`                | Toggles fly/no-collision mode.                                               |
| Movement   | Low gravity         | `gravity`               | Toggles moon-gravity (0.25x) for the target.                                 |
| Movement   | Freeze              | `freeze`                | Toggles movement lock.                                                       |
| Movement   | Random teleport     | `random_teleport`      | Drops the target on top of a random other living player.                     |
| Movement   | Swap positions       | `swap_positions`       | Swaps the target and a random other living player instantly.                 |
| Movement   | Launch               | `launch`                | Sends the target flying with a velocity impulse.                             |
| Movement   | Teleport spam        | `teleport_spam`        | Repeats random teleport every few seconds until toggled off.                 |
| Combat     | Strip Weapons       | `stripweapons`         | Removes every weapon and grenade.                                            |
| Combat     | Kill Player         | `kill`                  | Instantly sets health to 0.                                                  |
| Combat     | Drop to 1 HP        | `one_hp`                | Sets health to 1 without killing them.                                       |
| Combat     | Knife only          | `disarm_to_knife`      | Removes everything except the knife (and the bomb, if carried).              |
| Combat     | Set on fire         | `ignite`                | Cosmetic ignite for a few seconds.                                           |
| Combat     | Godmode             | `godmode`               | Restores health after every hit until toggled off.                          |
| Combat     | Random weapon       | `random_weapon`        | Hands over a random gun from a fixed pool.                                   |
| Combat     | Armor (Full/Strip)  | `armor`                 | Sets armor to 100 or 0.                                                      |
| Combat     | Jammed gun          | `jammed_gun`            | Zeroes the clip after every shot, forcing a reload constantly.               |
| Visual     | Drunk vision        | `drunk_vision`          | Wobbles FOV for ~6 seconds, then resets it.                                  |
| Visual     | Invisible           | `invisible`             | Toggles full model transparency.                                             |
| Visual     | Earthquake          | `earthquake`            | Jitters the target's view angles for a few seconds.                          |
| Visual     | Spin                | `spin`                  | Continuously spins the target's view for a few seconds.                      |
| Visual     | Flashbang           | `flash`                 | Cosmetic instant flash-blind for a few seconds.                              |
| Visual     | Scale (Giant/Tiny)  | `scale`                 | Resizes the target's model; cosmetic only, hitbox unaffected.               |
| Visual     | Annoying sound      | `annoying_sound`        | Plays a configurable sound only the target can hear.                        |
| Visual     | Upside down         | `upside_down`           | Flips the target's camera roll 180 degrees.                                 |
| Visual     | Haze                | `hazy_vision`            | Permanent partial whiteout until toggled off.                               |
| Visual     | Permanent zoom       | `sniper_vision`        | Locks FOV zoomed in on any weapon.                                           |
| Visual     | Color tint          | `tint`                  | Recolors the model to a random bright color.                                |
| Connection | Connection problems | `connection_problems`  | Simulates jitter/desync for 5s. Two modes: with chat notice, or silent.      |
| Utility    | Toggle mute         | `mute`                  | Silently drops the target's `say`/`say_team` chat while active.             |
| Utility    | Money (Broke/Rich)   | `money`                | Sets money to 0 or a large amount.                                          |
| Anti-Cheat | Enemy data blackout | `blind_enemies`         | Strips enemy pawns from the target's network stream. Silent — see below.    |
| Anti-Cheat | Spectator lock       | `spectator_lock`       | Forces and holds the target in Spectator until toggled off.                  |
| Anti-Cheat | Decoy bot            | `decoy_bot`             | Spawns a bot visible only to the target (+ admin). Silent — see below.       |
| Streamer   | Block defuse         | `block_defuse`          | Silently cancels the target's bomb defuse. Silent — see below.               |
| Streamer   | Silent blind         | `stream_blind`          | Indefinite whiteout, zero chat output, until toggled off.                    |
| Streamer   | Stuck key (×4)       | `stuck_key`             | Snaps the target back in place while one movement key is held. Silent.       |

---

## Localization

The plugin ships English and Russian translations
(`lang/en.json`, `lang/ru.json`) following the standard CounterStrikeSharp
localization format. Every effect's menu label and chat messages are driven
by `troll.<effect id>` / `troll.<effect id>.chat` keys, plus a
`category.<category>` key per menu category — see the lang files for the
full key list.

---

## Extending the plugin

To add a brand-new effect:

1. Create a class in `Effects/BuiltIn/` implementing `ITrollEffect`
   (`Id`, `Category`, `GetVariants()`).
2. Register an instance of it in `AOGTrollPlugin.RegisterEffects()`.
3. Add its localization keys (`troll.<id>` and any `.chat` variants) to both
   `lang/en.json` and `lang/ru.json`.
4. That's it — it'll show up in its category's menu automatically, and get a
   default `true` entry in `config.json` the next time the config is parsed.

Effects that need to remember on/off state per player (like `noclip` or
`gravity`) should use the shared `TrollStateStore` (`ctx.State.Toggle(...)`)
rather than instance fields, so state stays consistent and gets cleared
together on `Unload()`.
