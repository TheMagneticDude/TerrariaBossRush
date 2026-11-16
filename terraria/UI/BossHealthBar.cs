using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace terraria.UI;

public class BossHealthBar
{
    private readonly Texture2D _pixel;
    private readonly Vector2 _position;

    private const int widthBar = 500;
    private const int heightBar = 22;
    private const int border = 2;

    private readonly Color _backgroundColor = new Color(35, 55, 75, 150);
    private readonly Color _healthColor = new Color(170, 55, 65, 230);
    private readonly Color _borderColor = new Color(120, 150, 175, 200);

    public BossHealthBar(Texture2D pixel, Vector2 position)
    {
        _pixel = pixel;
        _position = position;
    }

    public void Draw(SpriteBatch spriteBatch, int currentHealth, int maxHealth)
    {
        System.ArgumentNullException.ThrowIfNull(spriteBatch);

        if (maxHealth <= 0)
        {
            return;
        }

        float healthPercentage =
            (float)currentHealth / maxHealth;

        Rectangle background = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            widthBar,
            heightBar
        );

        spriteBatch.Draw(_pixel, background, _backgroundColor);

        Rectangle health = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            (int)(widthBar * healthPercentage),
            heightBar
        );

        spriteBatch.Draw(_pixel, health, _healthColor);
        DrawBorder(spriteBatch, background);
    }

    private void DrawBorder(SpriteBatch spriteBatch, Rectangle bar)
    {
        spriteBatch.Draw(_pixel,
            new Rectangle(bar.X, bar.Y, bar.Width, border), _borderColor);

        spriteBatch.Draw(_pixel,
            new Rectangle(bar.X, bar.Bottom - border, bar.Width, border), _borderColor);

        spriteBatch.Draw(_pixel,
            new Rectangle(bar.X, bar.Y, border, bar.Height), _borderColor);

        spriteBatch.Draw(_pixel,
            new Rectangle(bar.Right - border, bar.Y, border, bar.Height), _borderColor);
    }

}