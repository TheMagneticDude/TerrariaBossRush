using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
namespace terraria.Items;

public class Item
{
    public Vector2 Position { get; set; }

    public Sprite TheSprite { get; set; }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        TheSprite.Draw(spriteBatch, Position);
    }

    public Item(Sprite sprite, Vector2 location)
    {
        this.TheSprite = sprite;
        this.Position = location;
    }

}
