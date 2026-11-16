using System;

using Microsoft.Xna.Framework;

namespace terraria.Bosses.States;

public class CthulhuAggroState: IBossState
{
    private EyeOfCthulhu _eye;
    private float _timer;

    public CthulhuAggroState(EyeOfCthulhu eye)
    {
        _eye = eye;
    }

    public void Enter()
    {
        _timer = 0;
        _eye.SetAnimation("cthulhu-phase-2");
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _timer += elapsed;

        _eye.Position += new Vector2(-100 * elapsed, 0);

        if (_timer >= 1)
        {
            _eye.ChangeState(new CthulhuNormalState(_eye));
        }
    }

}