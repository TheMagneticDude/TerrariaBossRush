using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.UI;

public class ManaBar
{
    private Texture2D _star;
    private Vector2 _position;

    private const float Scale = 1.4f;
    private const int Stars = 10;

    public ManaBar(Texture2D star, Vector2 position)
    {
        _star = star;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch, int currentMana, int maxMana)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        int fullMana = (int)Math.Ceiling(
            (double)currentMana * Stars / maxMana
        );

        for (int ii = 0; ii < Stars; ii++)
        {
            Color starColor;
    
            if (ii < fullMana)
            {
                starColor = Color.White;
            }
            else
            {
                starColor = Color.DarkSlateBlue;
            }

            Vector2 starPosition = new Vector2(
                _position.X + ii * (_star.Width * Scale + 4),
                _position.Y
            );

            spriteBatch.Draw(
                _star,
                starPosition,
                null,
                starColor,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f
                );
        }
    }
}