using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    /// <summary>
    /// Streamer-prank effect: while active, the target's defuse attempts
    /// never complete - their key just stops doing anything. Implemented by
    /// hooking bomb_begindefuse as a Pre-hook and returning HookResult.Stop.
    ///
    /// Caveat worth testing before relying on this: game events in Source
    /// are primarily a broadcast/notification mechanism, and it's possible
    /// stopping this one only suppresses the HUD/sound broadcast rather than
    /// the underlying defuse timer the game already started server-side.
    /// This is the standard technique CS2 "no-defuse-zone"-style plugins use
    /// and it's reported to actually block the defuse, not just the event,
    /// but verify it on your own server before trusting it in a real bait.
    ///
    /// Deliberately silent, same as the anti-cheat effects: no message to
    /// the target. The whole point on a streamer is that their key
    /// mysteriously "stops working" with no server message to explain why -
    /// a chat line would show up on their own stream/overlay and blow it
    /// instantly.
    /// </summary>
    public sealed class BlockDefuseEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "block_defuse";
        public EffectCategory Category => EffectCategory.Streamer;

        public BlockDefuseEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.RegisterEventHandler<EventBombBegindefuse>(OnBeginDefuse, HookMode.Pre);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.block_defuse", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowBlocked = ctx.State.Toggle(Id, ctx.Target.Slot);
            ctx.PrintToAdmin(nowBlocked ? "troll.block_defuse.on.chat" : "troll.block_defuse.off.chat");
        }

        private HookResult OnBeginDefuse(EventBombBegindefuse @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player is not { IsValid: true } || !_state.IsActive(Id, player.Slot))
                return HookResult.Continue;

            return HookResult.Stop;
        }
    }
}
