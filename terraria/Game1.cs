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
    private List<Texture2D> _backgroundTextures;
    private Rectangle _screenRectangle;

    private Texture2D _statusEffectPixel;
    private StatusEffectVisuals _statusEffectVisuals;
    private HotbarUI _hotbarUI;
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
        _worldHandler = new WorldHandler(WindowWidth, WindowHeight, _backgroundTextures);
        _screenRectangle = new Rectangle(0, 0, WindowWidth, WindowHeight);

        // Core.Initialize triggers LoadContent in this project, so all fields
        // needed by LoadContent must be initialized before this call.
        base.Initialize();
        blankTexture = new Texture2D(GraphicsDevice, 1, 1);
        DEBUG.LoadContent(GraphicsDevice, blankTexture);
    }

    protected override void LoadContent()
    {
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
            ItemFactory.Instance.CreateHealthPot(),
            ItemFactory.Instance.CreateRecallPot(),
            new WeaponHotbarEntry("Staff", WeaponFactory.Instance.CreateStaff(_projectileManager), false),
            new WeaponHotbarEntry("Yo-yo", WeaponFactory.Instance.CreateYoyo(), false),
            new WeaponHotbarEntry("Whip", WeaponFactory.Instance.CreateWhip(), false),
            new WeaponHotbarEntry("Fire Bow", WeaponFactory.Instance.CreateFireBow(_projectileManager), false),
            new WeaponHotbarEntry("Frost Staff", WeaponFactory.Instance.CreateFrostStaff(_projectileManager), false)
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

        _hotbarUI = new HotbarUI();

        _combatManager = new CombatManager(_projectileManager);
        _roomManager.RoomLoaded += _combatManager.Clear;
        _roomManager.LoadCurrentRoom();
    }

    protected override void UnloadContent()
    {
        _statusEffectPixel?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {

        base.Update(gameTime);

        _music.Update();

        // Input should update before anything reads it.
        _inputManager.Update(gameTime);
        _p1Input.Update(gameTime);
        _roomManager.Update(_p1Input, gameTime);

        _bossManager.UpdateAll(gameTime, _player1.Position);

        // Player may spawn projectiles during this update.
        _player1.Update(gameTime);
        _combatManager.Update(gameTime, _player1,_bossManager.Bosses, _roomManager.WorldMesh);

        _worldHandler.Update(_player1.Position);




        //exit handler
        if (_p1Input.IsNewPress(KeyAction.Exit))
        {
            Exit();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, null);
        // Draw using a source rectangle larger than the texture dimensions to trigger tiling
        Rectangle destinationRectangle = new Rectangle(-_backgroundTextures[_worldHandler.BackgroundID].Width, - (int) (_backgroundTextures[_worldHandler.BackgroundID].Height / 2f), 5000, 5000);
        Rectangle sourceRectangle = new Rectangle(-_backgroundTextures[_worldHandler.BackgroundID].Width, - (int) (_backgroundTextures[_worldHandler.BackgroundID].Height / 2f), 5000, 5000); 

        SpriteBatch.Draw(_backgroundTextures[_worldHandler.BackgroundID], destinationRectangle, sourceRectangle, Color.White);
        SpriteBatch.End();



        SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: _worldHandler.GetTransformMatrix()
        );


        _bossManager.DrawAll(SpriteBatch, _statusEffectVisuals);

        _player1.Draw(SpriteBatch);
        _projectileManager.Draw(SpriteBatch);
        _roomManager.Draw(SpriteBatch);

        SpriteBatch.End();

        SpriteBatch.Begin(
        samplerState: SamplerState.PointClamp
        );

        _hotbarUI.Draw(SpriteBatch, _player1.Hotbar);

        if (_player1.IsDead)
        {
            DeathOverlay.Draw(SpriteBatch, blankTexture);
        }

        SpriteBatch.End();

        

        base.Draw(gameTime);
    }
}
