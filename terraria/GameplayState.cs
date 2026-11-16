using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Input;
using terraria.Blocks;
using terraria.Combat;
using terraria.Bosses;
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

internal sealed class GameplayState : IDisposable
{
    private BossManager _bossManager;

    private readonly InputManager _inputManager;
    private IPlayer _player1;
    private readonly PlayerInput _p1Input;

    private MusicPlayer _music;

    private WorldHandler _worldHandler;
    private List<Texture2D> _backgroundTextures;

    private UIManager _uiManager;
    private GumUIManager _gumUIManager;
    private Texture2D _statusEffectPixel;
    private StatusEffectVisuals _statusEffectVisuals;
    private CombatManager _combatManager;
    private RoomManager _roomManager;

    private readonly ProjectileManager _projectileManager = new ProjectileManager();

    private readonly Action _exit;

    public GameplayState(Action exit)
    {
        _exit = exit;
        _inputManager = new InputManager();
        _p1Input = new PlayerInput(_inputManager);
    }

    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, Game game, Action onExitGame)
    {
        LoadVisuals(content, graphicsDevice);
        LoadEntityTextures(content);
        _bossManager = new BossManager();
        _player1 = PlayerFactory.Create(content, _p1Input, _projectileManager);
        BossFactory.Instance.TargetPlayer = _player1;
        BossFactory.Instance.Projectiles = _projectileManager;
        StartMusic(content);
        _uiManager = new UIManager(content);
        InitializeRooms();

        _gumUIManager = new GumUIManager(game, _p1Input, onExitGame);
        _gumUIManager.Initialize();
    }

    private void LoadVisuals(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _backgroundTextures = [content.Load<Texture2D>("Background/Forestbackground")];
        _worldHandler = new WorldHandler(WindowWidth, WindowHeight, _backgroundTextures);
        _statusEffectPixel = new Texture2D(graphicsDevice, 1, 1);
        _statusEffectPixel.SetData(new[] { Color.White });
        _statusEffectVisuals = new StatusEffectVisuals(_statusEffectPixel);
    }

    private void LoadEntityTextures(ContentManager content)
    {
        BlockFactory.Instance.LoadAllTextures(content);
        BossFactory.Instance.LoadAllTextures(content);
        ItemFactory.Instance.LoadAllTextures(content);
        ProjectileFactory.Instance.LoadAllTextures(content);
        WeaponFactory.Instance.LoadAllTextures(content, _statusEffectPixel);
    }

    private void InitializeRooms()
    {
        _roomManager = new RoomManager(new RoomFactory(), _bossManager, _player1);
        _combatManager = new CombatManager(_projectileManager);
        _roomManager.RoomLoaded += _combatManager.Clear;
        _roomManager.LoadCurrentRoom();
    }

    private void StartMusic(ContentManager content)
    {
        List<Song> playList =
        [
            content.Load<Song>("Music/Overworld/Music-Overworld_Day"),
            content.Load<Song>("Music/Overworld/Music-Overworld_Night"),
            content.Load<Song>("Music/Overworld/Music-Underground"),
        ];

        _music = new MusicPlayer(playList, false, 0.5f);
        _music.Start();
    }

    public void Update(GameTime gameTime)
    {
        _music.Update();

        // Input should update before anything reads it.
        _inputManager.Update(gameTime);
        _p1Input.Update(gameTime);

        _gumUIManager.Update(gameTime,_inputManager);

        if (!_gumUIManager.IsGamePaused)
        {//only run updates when unpaused
            _roomManager.Update(_p1Input, gameTime);

            _bossManager.UpdateAll(gameTime, _player1.Position);

            // Player may spawn projectiles during this update.
            _player1.Update(gameTime);
            _combatManager.Update(gameTime, _player1, _bossManager.Bosses, _roomManager.WorldMesh);

            _worldHandler.Update(_player1.Position);
        }
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        graphicsDevice.Clear(Color.CornflowerBlue);

        DrawBackground(spriteBatch);


        if (_gumUIManager.GameStarted)
        {

            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: _worldHandler.GetTransformMatrix()
            );

            _bossManager.DrawAll(spriteBatch, _statusEffectVisuals);

            _player1.Draw(spriteBatch);
            _projectileManager.Draw(spriteBatch);
            _roomManager.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(
            samplerState: SamplerState.PointClamp
            );

            _uiManager.Draw(spriteBatch, _player1, _bossManager);

            spriteBatch.End();
        }

        _gumUIManager.Draw();
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        Texture2D background = _backgroundTextures[_worldHandler.BackgroundID];
        Rectangle tiledArea = new Rectangle(-background.Width, -(int)(background.Height / 2f), 5000, 5000);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
        // A source rectangle larger than the texture triggers tiling.
        spriteBatch.Draw(background, tiledArea, tiledArea, Color.White);
        spriteBatch.End();
    }

    public void Dispose()
    {
        if (_roomManager != null && _combatManager != null)
        {
            _roomManager.RoomLoaded -= _combatManager.Clear;
        }
        _statusEffectPixel?.Dispose();
    }
}
