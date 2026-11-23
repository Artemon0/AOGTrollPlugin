---
layout: default
title: Commands and permissions
---

# Commands and permissions

- **`css_atroll`** — Opens the player selection menu. Restricted to admins
  holding both `@css/troll` and the flag configured in `RequiresFlag`.

Menu flow: **pick a player → pick a category → pick an effect** (effects
with more than one mode, like Connection Problems, open one extra sub-menu).

**Utility → Random effect** draws an enabled effect and then one of its variants
with equal chances at each step. The draw excludes itself, Respawn, Anti-Cheat,
and Streamer Pranks. Set `Effects.random` to `false` to hide it.

## Chat command

Use `!atroll` in chat or `css_atroll` in the console. Both require `@css/troll` and any additional configured flag.
