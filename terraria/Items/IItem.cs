using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Hotbar;
using terraria.Players;

namespace terraria.Items;

public interface IItem
{
    bool IsActive { get; }
    void Use(IPlayer player);
    void Update(GameTime gameTime, IPlayer player);
    void Draw(SpriteBatch spriteBatch, IPlayer player);
    void DrawIcon(SpriteBatch spriteBatch, Rectangle slot);
}
