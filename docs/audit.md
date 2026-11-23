---
layout: default
title: Audit logging
---

# Audit logging

The plugin logs effect invocations to the console and to daily files under
`logs/` (using `AOGTrollPlugin.log` as the base filename). Entries include the
admin, target, Steam IDs, effect variant, map, and whether the effect was picked
directly or at random. An entry records the invocation, even when the effect's
own conditions prevent a change.

To copy these entries to Discord, create an incoming webhook for each destination
channel and add the copied URLs to the plugin's `config.json`:

```json
"DiscordWebhookUrls": [
  "https://discord.com/api/webhooks/WEBHOOK_ID/WEBHOOK_TOKEN"
]
```

Use `[]` for local logging only. Keep webhook URLs private. Discord delivery
runs in the background. Failed deliveries or a full queue leave the local log
intact. Pending Discord messages may be dropped when the plugin unloads.
