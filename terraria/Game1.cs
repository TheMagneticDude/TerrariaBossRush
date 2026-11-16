using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using static terraria.Constants;

namespace terraria;

public class Game1 : Core
{
    private GameplayState _gameplay;
    //================Boss init===========================
    private BossManager _bossManager;

    //================Player init===========================
    private InputManager _inputManager;
    private IPlayer _player1;
    private PlayerInput _p1Input;

    //================Music init===========================
    private MusicPlayer _music;
    private List<Song> _playList;

    //================World Handling===========================
    private WorldHandler _worldHandler;
    private List<Texture2D> _backgroundTextures;
    private Rectangle _screenRectangle;

    private UIManager _uiManager;
    private Texture2D _statusEffectPixel;
    private StatusEffectVisuals _statusEffectVisuals;
    private CombatManager _combatManager;
    private RoomManager _roomManager;

    //================Projectiles===========================
    private readonly ProjectileManager _projectileManager = new ProjectileManager();


    //================Debug===========================
    public static Texture2D blankTexture;

    public Game1() : base(AppName, WindowWidth, WindowHeight, false)
    {
    }

    protected override void Initialize()
    {
        // Core.Initialize triggers LoadContent, so create the state first.
        _gameplay = new GameplayState(Exit);
        base.Initialize();
        blankTexture = new Texture2D(GraphicsDevice, 1, 1);
        DEBUG.LoadContent(GraphicsDevice, blankTexture);
    }

    protected override void LoadContent()
    {
        // Core assigns its GraphicsDevice after base.Initialize returns.
        _gameplay.LoadContent(Content, Graphics.GraphicsDevice);
        _statusEffectPixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
        _statusEffectPixel.SetData(new[] { Color.White });
        _statusEffectVisuals = new StatusEffectVisuals(_statusEffectPixel);

        BlockFactory.Instance.LoadAllTextures(Content);
        BossFactory.Instance.LoadAllTextures(Content);
        

        _bossManager = new BossManager();
        

        ItemFactory.Instance.LoadAllTextures(Content);
        ProjectileFactory.Instance.LoadAllTextures(Content);
        WeaponFactory.Instance.LoadAllTextures(Content, _statusEffectPixel);


        TextureAtlas playerAtlas = TextureAtlas.FromFile(Content, "Player/Player.xml");

        IHotbarEntry[] hotbarEntries =
        [
            new WeaponHotbarEntry("Wooden Sword", WeaponFactory.Instance.CreateSword(), false),
            new WeaponHotbarEntry("Magic Dagger", WeaponFactory.Instance.CreateDagger(_projectileManager), false),
            new WeaponHotbarEntry("Bow", WeaponFactory.Instance.CreateBow(_projectileManager), false),
            new ItemHotbarEntry("Health Potion", ItemFactory.Instance.CreateHealthPot()),
            new ItemHotbarEntry("Recall Potion", ItemFactory.Instance.CreateRecallPot()),
            new ItemHotbarEntry("Mana Potion", ItemFactory.Instance.CreateManaPot()),
            new ItemHotbarEntry("Swiftness Potion", ItemFactory.Instance.CreateSwiftPot()),
            new WeaponHotbarEntry("Staff", WeaponFactory.Instance.CreateStaff(_projectileManager), false),
            new WeaponHotbarEntry("Yo-yo", WeaponFactory.Instance.CreateYoyo(), false),
            new WeaponHotbarEntry("Whip", WeaponFactory.Instance.CreateWhip(), false),
            //new WeaponHotbarEntry("Fire Bow", WeaponFactory.Instance.CreateFireBow(_projectileManager), false),
            //new WeaponHotbarEntry("Frost Staff", WeaponFactory.Instance.CreateFrostStaff(_projectileManager), false)
        ];

        PlayerHotbar hotbar = new PlayerHotbar(hotbarEntries);
        _player1 = new Player(playerAtlas.CreateAnimatedSprite("Idle"), playerAtlas, hotbar, _p1Input);

        BossFactory.Instance.TargetPlayer = _player1;

        RoomFactory roomFactory = new RoomFactory();
        _roomManager = new RoomManager(roomFactory,_bossManager,_player1);

        _playList =
        [
            Content.Load<Song>("Music/Overworld/Music-Overworld_Day"),
            Content.Load<Song>("Music/Overworld/Music-Overworld_Night"),
            Content.Load<Song>("Music/Overworld/Music-Underground"),
        ];

        _music = new MusicPlayer(_playList, false, 0.5f);
        _music.Start();

        _backgroundTextures = [
            Content.Load<Texture2D>("Background/Forestbackground"),
        ];

        _uiManager = new UIManager(Content);

        _combatManager = new CombatManager(_projectileManager);
        _roomManager.RoomLoaded += _combatManager.Clear;
        _roomManager.LoadCurrentRoom();
    }

    protected override void UnloadContent()
    {
        _gameplay?.Dispose();
        blankTexture?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _gameplay.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _gameplay.Draw(GraphicsDevice, SpriteBatch);
        base.Draw(gameTime);
    }
}
