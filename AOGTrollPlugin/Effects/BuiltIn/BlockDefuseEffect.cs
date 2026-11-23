using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace AOGTrollPlugin.Effects.BuiltIn
{
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
