using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.Blocks;
public interface IBlock
{
    Vector2 Position { get; set; }
    
    void Update(GameTime gameTime);

    void Draw(SpriteBatch spriteBatch);
}