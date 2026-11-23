using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace AOGTrollPlugin.Effects.BuiltIn
{
    public sealed class MuteEffect : ITrollEffect
    {
        private readonly TrollStateStore _state;

        public string Id => "mute";
        public EffectCategory Category => EffectCategory.Utility;

        public MuteEffect(BasePlugin plugin, TrollStateStore state)
        {
            _state = state;
            plugin.AddCommandListener("say", OnChatCommand, HookMode.Pre);
            plugin.AddCommandListener("say_team", OnChatCommand, HookMode.Pre);
        }

        public IReadOnlyList<TrollEffectVariant> GetVariants() =>
        [
            new TrollEffectVariant("troll.mute", Run)
        ];

        private void Run(TrollEffectContext ctx)
        {
            bool nowMuted = ctx.State.Toggle(Id, ctx.Target.Slot);
            ctx.PrintToTarget(nowMuted ? "troll.mute.on.chat" : "troll.mute.off.chat");
        }

        private HookResult OnChatCommand(CCSPlayerController? player, CommandInfo info)
        {
            if (player is not { IsValid: true })
                return HookResult.Continue;

            return _state.IsActive(Id, player.Slot) ? HookResult.Stop : HookResult.Continue;
        }
    }
}
