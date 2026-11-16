using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using static terraria.Constants;

namespace terraria;

public class Game1 : Core
{
    private GameplayState _gameplay;

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
