---
layout: default
title: Configuration
---

# Configuration

Generated at `addons/counterstrikesharp/configs/plugins/AOGTrollPlugin/config.json`
on first launch:

| Parameter        | Type              | Default        | Description                                                        |
| ----------------- | ----------------- | -------------- | -------------------------------------------------------------------- |
| `EnableTrolling`  | Boolean           | `true`         | Master on/off switch for the whole plugin.                          |
| `RequiresFlag`    | String            | `"@css/troll"` | Additional admin flag required alongside `@css/troll`.  |
| `SoundEffectEvent`| String            | `"UI.WeaponPurchase"` | Soundevent name played by `annoying_sound` — swap it if the default doesn't play on your build. |
| `DiscordWebhookUrls` | Array of strings | `[]` | Optional Discord webhook URLs for copies of the admin action log. |
| `Effects`         | Object (bool map) | all `true`     | Per-effect toggle, keyed by effect id — see table below.            |

Example `Effects` block (trimmed):

```json
"Effects": {
  "skybox": true,
  "stripweapons": true,
  "kill": true,
  "one_hp": true,
  "health": true,
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
  "random_grenade": true,
  "ammo": true,
  "respawn": true,
  "random": true,
  "tint": true,
  "armor": true,
  "jammed_gun": true,
  "teleport_spam": true
}
```

Set any of these to `false` to remove that effect from the menu entirely —
reload the configuration or restart the server to apply changes.
