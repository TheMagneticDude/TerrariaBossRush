using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.UI;

public class HealthBar
{
    private Texture2D _heart;
    private Texture2D _emptyHeart;
    private Vector2 _position;

    private const float Scale = 1.4f;
    private const int Hearts = 10;

    public HealthBar(Texture2D heart, Texture2D emptyHeart, Vector2 position)
    {
        _heart = heart;
        _emptyHeart = emptyHeart;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch, int currentHealth, int maxHealth)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        int fullHearts = (int)Math.Ceiling(
            (double)currentHealth * Hearts / maxHealth
        );

        for (int ii = 0; ii < Hearts; ii++)
        {
            Texture2D texture; 
            if (ii < fullHearts)
            {
                texture = _heart;
            }
            else
            {
                texture = _emptyHeart;
            }

            Vector2 heartPosition = new Vector2(
                _position.X + ii * (_heart.Width * Scale + 4),
                _position.Y
            );

            //spriteBatch.Draw(texture, heartPosition, Color.White);
            spriteBatch.Draw(
                texture,
                heartPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f
                );
        }
    }
}