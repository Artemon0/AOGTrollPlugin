using AOGTrollPlugin.Effects;
using AOGTrollPlugin.Effects.BuiltIn;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using MenuManager;

namespace AOGTrollPlugin;

public partial class AOGTrollPlugin
{
        private void RegisterEffects()
        {
            _connectionProblems = new ConnectionProblemsEffect();

            _registry.Register(new SkyboxEffect());
            _registry.Register(new StripWeaponsEffect());
            _registry.Register(new KillEffect());
            _registry.Register(new OneHpEffect());
            _registry.Register(new HealthEffect());
            _registry.Register(new SlapEffect());
            _registry.Register(new DisarmToKnifeEffect());
            _registry.Register(new IgniteEffect());
            _registry.Register(_connectionProblems);
            _registry.Register(new NoclipEffect());
            _registry.Register(new GravityEffect());
            _registry.Register(new FreezeEffect());
            _registry.Register(new DrunkVisionEffect());
            _registry.Register(new InvisibleEffect());
            _registry.Register(new EarthquakeEffect());
            _registry.Register(new SpinEffect());
            _registry.Register(new FlashEffect());
            _registry.Register(new RandomTeleportEffect());
            _registry.Register(new MuteEffect(this, _state));
            _registry.Register(new ScaleEffect());
            _registry.Register(new GodmodeEffect(this, _state));
            _registry.Register(new SwapPositionsEffect());
            _registry.Register(new AnnoyingSoundEffect(Config));
            _registry.Register(new MoneyEffect());
            _registry.Register(new UpsideDownEffect());
            _registry.Register(new HazyVisionEffect());
            _registry.Register(new LaunchEffect());
            _registry.Register(new SniperVisionEffect());
            _registry.Register(new RandomWeaponEffect());
            _registry.Register(new TintEffect());
            _registry.Register(new ArmorEffect());
            _registry.Register(new JammedGunEffect(this, _state));
            _registry.Register(new TeleportSpamEffect());

            // Anti-cheat punishment effects - see their class docs for exactly
            // what each one does and does not defeat.
            _registry.Register(new BlindEnemiesEffect(this, _state));
            _registry.Register(new SpectatorLockEffect(this, _state));
            _registry.Register(new DecoyBotEffect(this, _state));

            // Streamer/YouTuber pranks - deliberately silent to the target,
            // see their class docs for why.
            _registry.Register(new BlockDefuseEffect(this, _state));
            _registry.Register(new StreamBlindEffect());
            _registry.Register(new StuckKeyEffect(this, _state));
        }

}
