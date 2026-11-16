using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.UI;

public class BuffIcon
{
    private readonly Texture2D _texture;
    private readonly Vector2 _position;

    public BuffIcon(Texture2D texture, Vector2 position)
    {
        ArgumentNullException.ThrowIfNull(texture);

        _texture = texture;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        spriteBatch.Draw(_texture, _position, Color.White);
    }
}