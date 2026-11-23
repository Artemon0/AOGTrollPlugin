---
layout: default
title: Effects reference
---

# Effects reference

Browse the available effects by category. Each ID can be enabled or disabled in [Configuration](configuration.html).

## Movement

| Effect | ID | Description |
| --- | --- | --- |
| Teleport to Skybox | `skybox` | Teleports the player 1000 units straight up. |
| Slap | `slap` | Classic random-direction knockback impulse. |
| Noclip | `noclip` | Toggles fly/no-collision mode. |
| Low gravity | `gravity` | Toggles moon-gravity (0.25x) for the target. |
| Freeze | `freeze` | Toggles movement lock. |
| Random teleport | `random_teleport` | Drops the target on top of a random other living player. |
| Swap positions | `swap_positions` | Swaps the target and a random other living player instantly. |
| Launch | `launch` | Sends the target flying with a velocity impulse. |
| Teleport spam | `teleport_spam` | Repeats random teleport every few seconds until toggled off. |

## Combat

| Effect | ID | Description |
| --- | --- | --- |
| Strip Weapons | `stripweapons` | Removes every weapon and grenade. |
| Kill Player | `kill` | Instantly sets health to 0. |
| Drop to 1 HP | `one_hp` | Sets health to 1 without killing them. |
| Knife only | `disarm_to_knife` | Removes everything except the knife (and the bomb, if carried). |
| Set on fire | `ignite` | Cosmetic ignite for a few seconds. |
| Godmode | `godmode` | Restores health after every hit until toggled off. |
| Random weapon | `random_weapon` | Hands over a random gun from a fixed pool. |
| Random grenade | `random_grenade` | Gives a random HE, flashbang, smoke, or decoy to a living player. |
| Active weapon ammo | `ammo` | Refills or empties the magazine and reserve ammo of the held firearm. |
| Armor (Full/Strip) | `armor` | Sets armor to 100 or 0. |
| Jammed gun | `jammed_gun` | Zeroes the clip after every shot, forcing a reload constantly. |
| Health | `health` | Sets health to 25, 100 or 200 HP. |

## Visual

| Effect | ID | Description |
| --- | --- | --- |
| Drunk vision | `drunk_vision` | Wobbles FOV for ~6 seconds, then resets it. |
| Invisible | `invisible` | Toggles full model transparency. |
| Earthquake | `earthquake` | Jitters the target's view angles for a few seconds. |
| Spin | `spin` | Continuously spins the target's view for a few seconds. |
| Flashbang | `flash` | Cosmetic instant flash-blind for a few seconds. |
| Scale (Giant/Tiny) | `scale` | Resizes the target's model; cosmetic only, hitbox unaffected. |
| Annoying sound | `annoying_sound` | Plays a configurable sound only the target can hear. |
| Upside down | `upside_down` | Flips the target's camera roll 180 degrees. |
| Haze | `hazy_vision` | Permanent partial whiteout until toggled off. |
| Permanent zoom | `sniper_vision` | Locks FOV zoomed in on any weapon. |
| Color tint | `tint` | Recolors the model to a random bright color. |

## Connection

| Effect | ID | Description |
| --- | --- | --- |
| Connection problems | `connection_problems` | Simulates jitter/desync for 5s. Two modes: with chat notice, or silent. |

## Utility

| Effect | ID | Description |
| --- | --- | --- |
| Toggle mute | `mute` | Silently drops the target's `say`/`say_team` chat while active. |
| Respawn player | `respawn` | Respawns a dead player on the T or CT team when selected manually. |
| Money (Broke/Rich) | `money` | Sets money to 0 or a large amount. |
| Random effect | `random` | Selects an eligible enabled effect and variant. |

## Anti-Cheat

| Effect | ID | Description |
| --- | --- | --- |
| Enemy data blackout | `blind_enemies` | Strips enemy pawns from the target's network stream. Silent. |
| Spectator lock | `spectator_lock` | Forces and holds the target in Spectator until toggled off. |
| Decoy bot | `decoy_bot` | Spawns a bot visible only to the target (+ admin). Silent. |

## Streamer

| Effect | ID | Description |
| --- | --- | --- |
| Block defuse | `block_defuse` | Silently cancels the target's bomb defuse. Silent. |
| Silent blind | `stream_blind` | Indefinite whiteout, zero chat output, until toggled off. |
| Stuck key (×4) | `stuck_key` | Snaps the target back in place while one movement key is held. Silent. |


