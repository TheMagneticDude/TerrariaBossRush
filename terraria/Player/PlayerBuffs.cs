using System;

using Microsoft.Xna.Framework;

namespace terraria.Players;

public class PlayerBuffs
{
    private float _swiftnessTimer;

    public bool HasSwiftness => _swiftnessTimer > 0f;

    public float MovementMultiplier
    {
        get
        {
            if (HasSwiftness)
            {
                return 1.5f;
            }

            return 1f;
        }
    }

    public void Swiftness(float duration)
    {
        _swiftnessTimer = duration;
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        
        if (_swiftnessTimer > 0f)
        {
            _swiftnessTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_swiftnessTimer < 0f)
            {
                _swiftnessTimer = 0f;
            }
        }
    }
}