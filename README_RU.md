# AOGTrollPlugin

Плагин административных эффектов для Counter-Strike 2 и CounterStrikeSharp. [English](README.md) · [Документация](https://artemon0.github.io/AOGTrollPlugin/)

## Установка

1. Установите [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) версии 1.0.374 или новее, [PlayerSettingsCS2](https://github.com/NickFox007/PlayerSettingsCS2) версии 0.9.4 или новее и [MenuManagerCS2](https://github.com/karola3vax/MenuManagerCS2/releases/latest).
2. Убедитесь, что `MenuManagerApi.dll` из MenuManager находится в `addons/counterstrikesharp/shared/MenuManagerApi/`.
3. Скачайте [последний релиз](https://github.com/Artemon0/AOGTrollPlugin/releases/latest) и распакуйте папку `AOGTrollPlugin` в `addons/counterstrikesharp/plugins/`.
4. Перезапустите сервер или смените карту для создания конфигурации.

## Настройка

Откройте `addons/counterstrikesharp/configs/plugins/AOGTrollPlugin/config.json`:

- `EnableTrolling` — включение плагина.
- `RequiresFlag` — необходимый флаг администратора; по умолчанию `@css/troll`.
- `SoundEffectEvent` — звуковое событие для эффекта раздражающего звука.
- `DiscordWebhookUrls` — необязательный массив URL вебхуков Discord; храните их в секрете.
- `Effects` — включение и отключение отдельных эффектов по их ID.

Для открытия меню используйте `css_atroll` или `!atroll`. Администратору нужны `@css/troll` и флаг из `RequiresFlag`, если он отличается. Список эффектов и подробности находятся в [документации](https://artemon0.github.io/AOGTrollPlugin/).

## Сборка

Установите .NET 10 SDK и выполните `dotnet build AOGTrollPlugin/AOGTrollPlugin.csproj -c Release`. В проект включён контракт API из [MenuManagerCS2](https://github.com/karola3vax/MenuManagerCS2), коммит `0c38f7c8f5ebf05d14b18e9e974cf38af9c4b071`; на сервере требуется общая API-библиотека из MenuManager.
