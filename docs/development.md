---
layout: default
title: Development
---

# Development

## Build from source

Build with `dotnet build AOGTrollPlugin/AOGTrollPlugin.csproj -c Release` using .NET 10. The API contract is included in `vendor/MenuManagerApi`; use the shared API distributed with MenuManager at runtime.

## Localization

The plugin ships English and Russian translations
(`lang/en.json`, `lang/ru.json`) following the standard CounterStrikeSharp
localization format. Every effect's menu label and chat messages are driven
by `troll.<effect id>` / `troll.<effect id>.chat` keys, plus a
`category.<category>` key per menu category — see the lang files for the
full key list.

## Adding an effect

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
