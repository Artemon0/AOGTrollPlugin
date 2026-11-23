using CounterStrikeSharp.API;
using AOGTrollPlugin.Effects;
using AOGTrollPlugin.Effects.BuiltIn;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using MenuManager;

namespace AOGTrollPlugin;

public partial class AOGTrollPlugin
{
        private void OpenPlayerMenu(CCSPlayerController admin)
        {
            string prefix = Localizer["troll.chat.prefix"];
            List<CCSPlayerController> players = GetCachedPlayers();
            IMenuApi? menuApi = _menuApi;

            if (menuApi != null)
            {
                IMenu playerMenu = menuApi.GetMenu(Localizer["player.select"]);
                playerMenu.PostSelectAction = PostSelectAction.Close;

                foreach (CCSPlayerController target in players)
                {
                    playerMenu.AddMenuOption(target.PlayerName, (a, _) => Server.NextFrame(() => { if (a.IsValid && target.IsValid) OpenCategoryMenu(a, target, prefix); }));
                }

                playerMenu.Open(admin);
                return;
            }

            CenterHtmlMenu legacyMenu = new(Localizer["player.select"], this);
            foreach (CCSPlayerController target in players)
            {
                legacyMenu.AddMenuOption(target.PlayerName, (a, _) => Server.NextFrame(() => { if (a.IsValid && target.IsValid) OpenCategoryMenu(a, target, prefix); }));
            }

            CounterStrikeSharp.API.Modules.Menu.MenuManager.OpenCenterHtmlMenu(this, admin, legacyMenu);
        }

        private void OpenCategoryMenu(CCSPlayerController admin, CCSPlayerController target, string prefix)
        {
            string title = $"{Localizer["player.troll"]}: {target.PlayerName}";
            List<IGrouping<EffectCategory, ITrollEffect>> groups = _registry.EnabledByCategory(Config).ToList();
            IMenuApi? menuApi = _menuApi;

            if (menuApi != null)
            {
                IMenu categoryMenu = menuApi.GetMenu(title, backAction: a => Server.NextFrame(() => { if (a.IsValid) OpenPlayerMenu(a); }));
                categoryMenu.PostSelectAction = PostSelectAction.Close;

                foreach (IGrouping<EffectCategory, ITrollEffect> group in groups)
                {
                    categoryMenu.AddMenuOption(
                        Localizer[$"category.{group.Key.ToString().ToLowerInvariant()}"],
                        (a, _) => Server.NextFrame(() => { if (a.IsValid && target.IsValid) OpenEffectMenu(a, target, prefix, group.Key, group.ToList()); })
                    );
                }

                categoryMenu.Open(admin);
                return;
            }

            CenterHtmlMenu legacyMenu = new(title, this);
            foreach (IGrouping<EffectCategory, ITrollEffect> group in groups)
            {
                legacyMenu.AddMenuOption(
                    Localizer[$"category.{group.Key.ToString().ToLowerInvariant()}"],
                    (a, _) => Server.NextFrame(() => { if (a.IsValid && target.IsValid) OpenEffectMenu(a, target, prefix, group.Key, group.ToList()); })
                );
            }

            CounterStrikeSharp.API.Modules.Menu.MenuManager.OpenCenterHtmlMenu(this, admin, legacyMenu);
        }

        private void OpenEffectMenu(
            CCSPlayerController admin,
            CCSPlayerController target,
            string prefix,
            EffectCategory category,
            IReadOnlyList<ITrollEffect> effects
        )
        {
            string title = Localizer[$"category.{category.ToString().ToLowerInvariant()}"];
            IMenuApi? menuApi = _menuApi;

            if (menuApi != null)
            {
                IMenu effectMenu = menuApi.GetMenu(title, backAction: a => Server.NextFrame(() => { if (a.IsValid && target.IsValid) OpenCategoryMenu(a, target, prefix); }));
                effectMenu.PostSelectAction = PostSelectAction.Close;

                foreach (ITrollEffect effect in effects)
                {
                    IReadOnlyList<TrollEffectVariant> variants = effect.GetVariants();

                    if (variants.Count == 1)
                    {
                        TrollEffectVariant variant = variants[0];
                        effectMenu.AddMenuOption(
                            Localizer[variant.LabelLocalizationKey],
                            (a, _) => Execute(effect, variant, a, target, prefix)
                        );
                        continue;
                    }

                    effectMenu.AddMenuOption(
                        Localizer[$"troll.{effect.Id}"],
                        (a, _) => Server.NextFrame(() =>
                        {
                            if (!a.IsValid || !target.IsValid) return;
                            IMenu variantMenu = menuApi.GetMenu(
                                Localizer[$"troll.{effect.Id}"],
                                backAction: player => Server.NextFrame(() => { if (player.IsValid && target.IsValid) OpenEffectMenu(player, target, prefix, category, effects); })
                            );
                            variantMenu.PostSelectAction = PostSelectAction.Close;

                            foreach (TrollEffectVariant variant in variants)
                            {
                                variantMenu.AddMenuOption(
                                    Localizer[variant.LabelLocalizationKey],
                                    (a2, _) => Execute(effect, variant, a2, target, prefix)
                                );
                            }

                            variantMenu.Open(a);
                        })
                    );
                }

                effectMenu.Open(admin);
                return;
            }

            CenterHtmlMenu legacyEffectMenu = new(title, this);
            foreach (ITrollEffect effect in effects)
            {
                IReadOnlyList<TrollEffectVariant> variants = effect.GetVariants();

                if (variants.Count == 1)
                {
                    TrollEffectVariant variant = variants[0];
                    legacyEffectMenu.AddMenuOption(
                        Localizer[variant.LabelLocalizationKey],
                        (a, _) => Execute(effect, variant, a, target, prefix)
                    );
                    continue;
                }

                legacyEffectMenu.AddMenuOption(
                    Localizer[$"troll.{effect.Id}"],
                    (a, _) =>
                    {
                        CenterHtmlMenu variantMenu = new(Localizer[$"troll.{effect.Id}"], this);
                        foreach (TrollEffectVariant variant in variants)
                        {
                            variantMenu.AddMenuOption(
                                Localizer[variant.LabelLocalizationKey],
                                (a2, _) => Execute(effect, variant, a2, target, prefix)
                            );
                        }

                        CounterStrikeSharp.API.Modules.Menu.MenuManager.OpenCenterHtmlMenu(this, a, variantMenu);
                    }
                );
            }

            CounterStrikeSharp.API.Modules.Menu.MenuManager.OpenCenterHtmlMenu(this, admin, legacyEffectMenu);
        }

}
