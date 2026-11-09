using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using terraria.Blocks;
using terraria.Combat;
using terraria.Bosses;
using terraria.Hotbar;
using terraria.Items;
using terraria.Music;
using terraria.Players;
using terraria.Projectiles;
using terraria.Weapons;
using terraria.World;
using terraria.Rooms;
using terraria.UI;
using static terraria.Constants;

namespace terraria;

public class Game1 : Core
{
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
    private Texture2D _backgroundTexture;
    private Rectangle _screenRectangle;

    private HotbarUI _hotbarUI;
    private HealthBar _healthBar;
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
        _inputManager = new InputManager();
        _p1Input = new PlayerInput(_inputManager);
        _worldHandler = new WorldHandler(WindowWidth, WindowHeight);
        _screenRectangle = new Rectangle(0, 0, WindowWidth, WindowHeight);

        // Core.Initialize triggers LoadContent in this project, so all fields
        // needed by LoadContent must be initialized before this call.
        base.Initialize();
        blankTexture = new Texture2D(GraphicsDevice, 1, 1);
        DEBUG.LoadContent(GraphicsDevice, blankTexture);
    }

    protected override void LoadContent()
    {
        BlockFactory.Instance.LoadAllTextures(Content);
        BossFactory.Instance.LoadAllTextures(Content);

        _bossManager = new BossManager();

        ItemFactory.Instance.LoadAllTextures(Content);
        ProjectileFactory.Instance.LoadAllTextures(Content);
        WeaponFactory.Instance.LoadAllTextures(Content);


        // Will create a class that automatically handles player creation later.
        TextureAtlas playerAtlas = TextureAtlas.FromFile(Content, "Player/Player.xml");

        IHotbarEntry[] hotbarEntries =
        [
            new WeaponHotbarEntry("Sword", WeaponFactory.Instance.CreateSword(), false),
            new WeaponHotbarEntry("Dagger", WeaponFactory.Instance.CreateDagger(_projectileManager), false),
            new EmptyHotbarEntry("Empty"),
            ItemFactory.Instance.CreateHealthPot(),
            ItemFactory.Instance.CreateRecallPot()
        ];

        PlayerHotbar hotbar = new PlayerHotbar(hotbarEntries);
        _player1 = new Player(playerAtlas.CreateAnimatedSprite("Idle"), playerAtlas, hotbar);

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

        _backgroundTexture = Content.Load<Texture2D>("Background/Forestbackground");

        _hotbarUI = new HotbarUI();
        Texture2D heart = Content.Load<Texture2D>("UI/heart");
        Texture2D emptyHeart = Content.Load<Texture2D>("UI/empty_heart");
        _healthBar = new HealthBar(heart, emptyHeart, new Vector2(925,20));

        _combatManager = new CombatManager(_projectileManager);
        _roomManager.RoomLoaded += _combatManager.Clear;
        _roomManager.LoadCurrentRoom();
    }

    protected override void Update(GameTime gameTime)
    {

        base.Update(gameTime);

        _music.Update();

        // Input should update before anything reads it.
        _inputManager.Update(gameTime);
        _p1Input.Update(gameTime);
        _roomManager.Update(_p1Input, gameTime);

        _bossManager.UpdateAll(gameTime, _player1.Location);

        // Player may spawn projectiles during this update.
        _player1.Update(gameTime, _p1Input);
        _combatManager.Update(gameTime, _player1,_bossManager.Bosses, _roomManager.WorldMesh);

        _worldHandler.Update(_player1.Location);




        //exit handler
        if (_p1Input.IsNewPress(KeyAction.Exit))
        {
            Exit();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: _worldHandler.GetTransformMatrix()
        );

        SpriteBatch.Draw(_backgroundTexture, _screenRectangle, Color.White);

        _bossManager.DrawAll(SpriteBatch);

        _player1.Draw(SpriteBatch);
        _projectileManager.Draw(SpriteBatch);
        _roomManager.Draw(SpriteBatch);

        SpriteBatch.End();

        SpriteBatch.Begin(
        samplerState: SamplerState.PointClamp
        );

        _hotbarUI.Draw(SpriteBatch, _player1.Hotbar);
        _healthBar.Draw(SpriteBatch, _player1.CurrentHealth, _player1.MaxHealth);

        if (_player1.IsDead)
        {
            DeathOverlay.Draw(SpriteBatch, blankTexture);
        }

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
