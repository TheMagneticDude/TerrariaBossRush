using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;

namespace terraria.UI;

public sealed class StatusEffectVisuals
{
    private const int ParticleCount = 8;
    private readonly Texture2D _pixel;

    public StatusEffectVisuals(Texture2D pixel)
    {
        _pixel = pixel ?? throw new ArgumentNullException(nameof(pixel));
    }

    public void Draw(SpriteBatch batch, Rectangle bounds, StatusEffectComponent effects)
    {
        ArgumentNullException.ThrowIfNull(batch);
        ArgumentNullException.ThrowIfNull(effects);
        bool burning = effects.Has(StatusEffectType.Burning);
        bool chilled = effects.Has(StatusEffectType.Slow);
        if (!burning && !chilled)
        {
            return;
        }

        if (burning)
        {
            DrawFire(batch, bounds, effects.AnimationTimeSeconds);
        }
        if (chilled)
        {
            DrawIce(batch, bounds, effects.AnimationTimeSeconds);
        }
    }

    private void DrawFire(SpriteBatch batch, Rectangle bounds, float time)
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            float progress = GetParticleProgress(time, i);
            int x = GetParticleX(bounds, i);
            int y = (int)(bounds.Bottom - progress * (bounds.Height + 12));
            DrawSpark(batch, x, y, 1 - progress);
        }
    }

    private void DrawIce(SpriteBatch batch, Rectangle bounds, float time)
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            int x = GetParticleX(bounds, i);
            int y = (int)(bounds.Top + GetParticleProgress(time, i) * bounds.Height);
            DrawSnowflake(batch, x, y);
        }
    }

    private void DrawSpark(SpriteBatch batch, int x, int y, float opacity)
    {
        batch.Draw(_pixel, new Rectangle(x, y, 3, 6), Color.OrangeRed * opacity);
        batch.Draw(_pixel, new Rectangle(x, y + 2, 2, 3), Color.Gold * opacity);
    }

    private void DrawSnowflake(SpriteBatch batch, int x, int y)
    {
        batch.Draw(_pixel, new Rectangle(x - 2, y, 5, 1), Color.LightCyan);
        batch.Draw(_pixel, new Rectangle(x, y - 2, 1, 5), Color.LightCyan);
    }

    private static float GetParticleProgress(float time, int index) => (time * 1.3f + index * 0.137f) % 1f;
    private static int GetParticleX(Rectangle bounds, int index) =>
        (int)(bounds.Left + bounds.Width * ((index * 0.37f) % 1f));
}
