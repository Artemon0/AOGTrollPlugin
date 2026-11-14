# Changelog

## What's new in 2.4.0

Ten more effects this round, still no new categories - `Utility`, `Visual`, `Movement`, `Combat`. Same rules as every previous batch: one `ITrollEffect` class per file, individually toggleable in `Effects`, fully localized RU/EN, all comments in English.

- **Money** (`money`, Utility, 2 variants: Broke / Rich) — sets `InGameMoneyServices.Account`. This one's the most solidly confirmed of the batch: it's the *exact* pattern from CounterStrikeSharp's own official TestPlugin, not something I pieced together from schema docs.
- **Upside down** (`upside_down`, Visual, toggle) — flips the target's camera roll 180 degrees.
- **Haze** (`hazy_vision`, Visual, toggle) — a permanent, visible partial whiteout (same mechanism as `flash`/`stream_blind`, lower alpha, and unlike `stream_blind` this one is not silent).
- **Launch** (`launch`, Movement) — sends the target flying with a velocity impulse.
- **Permanent zoom** (`sniper_vision`, Visual, toggle) — locks FOV zoomed in on any weapon.
- **Random weapon** (`random_weapon`, Combat) — hands over a random gun from a fixed pool.
- **Color tint** (`tint`, Visual, toggle) — recolors the model using the exact same `Render`/`RenderMode`/`SetStateChanged` mechanism `invisible` already uses, just at full alpha instead of 0.
- **Armor** (`armor`, Combat, 2 variants: Full / Strip) — sets `ArmorValue`.
- **Jammed gun** (`jammed_gun`, Combat, toggle) — zeroes the clip after every shot via a `weapon_fire` hook, forcing a reload constantly.
- **Teleport spam** (`teleport_spam`, Movement, toggle) — repeats `random_teleport` on a loop every few seconds until toggled off.

All ten print a normal chat line to the target — none of them are silent, unlike the anti-cheat/streamer-prank batches from earlier.

## What's new in 2.3.0

Six more effects, no new categories - these slot into `Visual`, `Combat`, `Movement`, and `Streamer Pranks`. Same rules as always: modular (`ITrollEffect`, one file each), individually toggleable in `Effects`, fully localized RU/EN.

- **Scale** (`scale`, Visual, 3 variants: Giant / Tiny / Reset) — resizes the target's model via `CGameSceneNode.AbsScale`. Purely cosmetic: Source 2 doesn't scale hitboxes with the model, so a giant's hittable area is still normal-sized. One thing to verify yourself after building: I'm confident `AbsScale` is a real field, less confident that `SetStateChanged(pawn, "CGameSceneNode", "m_flAbsScale")` is exactly the right way to flag a *nested* field (reached through `CBodyComponent`) as dirty for networking, versus other fields in this plugin that live directly on the pawn's own schema chain. If the resize doesn't show up for other clients, the class doc has a fallback to try.
- **Godmode** (`godmode`, Combat, toggle) — restores health after every hit via a `player_hurt` Post-hook, so hit sounds/blood/hitmarkers still play for whoever's shooting, only the health loss gets undone.
- **Swap positions** (`swap_positions`, Movement) — instantly swaps the target with a random other living player.
- **Annoying sound** (`annoying_sound`, Visual) — plays a sound only the target can hear via `EmitSound` + a `RecipientFilter` containing just them. The sound event name is a config value (`SoundEffectEvent`), not hardcoded — CS2 plays sounds by soundevent name from the game's `.vsndevts` files, and I can't confirm from here which event names exist on any given server build, so this is deliberately made easy to swap.
- **Stuck key** (`stuck_key`, Streamer Pranks, 4 variants: Forward/Back/Left/Right) — the follow-up to last round's "unbind a movement key" ask. Real client-side unbinding still isn't something a server can do (unchanged from `block_defuse`'s explanation), but this gets close for one direction: every tick, if the flagged button is held, the pawn gets snapped back to its last known position from before the key was pressed. Holding the blocked key with a different, still-working direction at the same time looks a little glitchy (the correction reverts the whole tick's movement, not just one axis) — a known, documented limitation, not a bug you need to chase.

Of this batch, only `stuck_key` is silent to the target (same reasoning as `block_defuse`) — `scale`, `godmode`, `swap_positions`, and `annoying_sound` all print a normal chat line like the rest of the general-purpose effects.

## What's new in 2.2.0

**Menu system:** See the current [installation instructions](README.md#installation-and-build) for the supported MenuManagerCS2 integration.

**Three new anti-cheat/streamer effects**, plus honest notes on two things I was asked for that I'm not confident I can deliver as described — see below.

- **Decoy bot** (`decoy_bot`, Anti-Cheat) — spawns a real bot on the enemy team and hides it from every client except the flagged target and the admin who deployed it (via the same `CheckTransmit` mechanism as `blind_enemies`). Because it's a genuine CS2 bot, the game's own AI moves it and shoots with it, and it's handed a real weapon — none of that is scripted. **What I couldn't deliver:** a colored outline that renders only on the admin's screen to mark it as the decoy. CounterStrikeSharp doesn't expose a confirmed way to render a glow to one specific observer — the game's own spotted-glow system is per-team, not per-player, so it can't be pointed at just the admin. What you get instead: a chat line to the admin naming which bot was just spawned, and since the admin is one of only two people who can see it at all, there's no ambiguity once you're looking at your own screen — just no special rendering on top of it.
- **Block defuse** (`block_defuse`, Streamer Pranks) — a real, implementable stand-in for "unbind their defuse key." **What I couldn't deliver:** literally forcing a client to run `unbind` (or any client-only console command) from the server. That's not a CounterStrikeSharp limitation — Source engine servers can't push client-side, config-mutating commands like `bind`/`unbind` to a client at all; it's the same reason `unbindall` shows up on every Source-family blocked-command list. `ExecuteClientCommandFromServer` only works for commands the server itself implements (`kill`, `noclip`, etc.), not client-local ones. What this effect does instead achieves the same practical outcome without touching anyone's local config: it silently cancels `bomb_begindefuse` for the flagged player, so their defuse key does nothing, this round, no chat message, no explanation — which is closer to what a stream prank needs anyway (their config isn't left broken after they disconnect). I did not build an equivalent for a single movement key (W/A/S/D) — reliably canceling one specific direction of input without freezing all movement needs lower-level access than I could confirm is exposed; `FreezeEffect` already covers the "movement doesn't work at all" version if that's close enough for your case, and I'm glad to keep digging on the single-key version if you want it specifically.
- **Silent blind** (`stream_blind`, Streamer Pranks) — a `flash`-style whiteout with no fixed duration and, unlike every other effect, **zero chat output to the target ever**. Toggle it on, it stays on (re-applied every second so it doesn't fade) until you toggle it off. Built for exactly the "quietly blind a streamer, no notification" case.

Both new categories (`AntiCheat`, `Streamer`) slot into the existing menu/config system automatically — no extra wiring needed to show them.

## What's new in 2.1.0

- Seven new effects (listed below), including a new **Anti-Cheat** category
  aimed specifically at suspected cheaters rather than general trolling:
  - **Enemy data blackout** (`blind_enemies`) — hooks `CheckTransmit` and
    strips every enemy pawn from the target's own network stream. Their
    client never receives enemy position/health/etc. data at all, while
    everyone else keeps seeing the target completely normally. This defeats
    any cheat reading from the same networked data the game client renders
    from (the overwhelming majority of public ESP/wallhack/radar cheats) —
    it can't do anything about a cheat fed from outside the game connection
    entirely. It's also the one effect in the whole plugin that stays
    **silent to the target on purpose**: telling a cheater their ESP just
    got cut is a hint to turn around and quit.
  - **Spectator lock** (`spectator_lock`) — forces the target to Spectator
    and reverts any team change (menu, `jointeam`, auto-balance — anything
    that fires `player_team`) back to Spectator on the next frame. It can't
    stop them disconnecting and reconnecting (nothing server-side can), so
    treat it as "can't play while connected," not a ban.
- General effects: **Drop to 1 HP**, **Spin** (continuous view spin),
  **Flashbang** (cosmetic instant flash), **Random teleport** (drops the
  target on a random other living player), **Toggle mute** (silently drops
  their `say`/`say_team` messages).

## What's new in 2.0.0

The plugin was rebuilt around a small **effect module system** instead of one
big command handler:

- Every troll effect is its own class implementing `ITrollEffect` under
  `Effects/BuiltIn/`. Adding a new effect no longer means touching the menu
  or command code — you write the class, register it once in `Load()`, and
  add its localization keys.
- Effects are grouped into categories (`Movement`, `Combat`, `Visual`,
  `Connection`, `Utility`, `AntiCheat`) and the in-game menu is now two
  levels deep (player → category → effect) so it stays readable as the
  effect list grows.
- Every effect can be individually enabled/disabled from `config.json` via
  the new `Effects` map, without touching code.
- Toggle-style effects (noclip, gravity, freeze, invisibility, mute,
  spectator lock, enemy data blackout) share a single `TrollStateStore`
  instead of ad-hoc fields, so their on/off state is handled consistently.
- Fixed a couple of bugs from 1.x: `stripweapons` printed one chat line per
  weapon removed instead of one total; `RequiresFlag` in the config was never
  actually checked at runtime (only the hardcoded attribute flag was).

**One thing was intentionally left out of the rewrite:** the old commented-out
"Aimbot" feature (auto-snapping a player's aim onto the nearest enemy on
weapon fire). That's real aim-assist logic — it gives whoever it's applied to
a genuine, functional aimbot, not just a cosmetic prank — so I didn't port or
extend it. Happy to help with anything else in the plugin.
