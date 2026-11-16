using System;

using Microsoft.Xna.Framework;

namespace terraria.Bosses.States;

public class CthulhuNormalState: IBossState
{
    private EyeOfCthulhu _eye;
    private float _timer;

    public CthulhuNormalState(EyeOfCthulhu eye)
    {
        _eye = eye;
    }

    public void Enter()
    {
        _timer = 0;
        _eye.SetAnimation("cthulhu-phase-1");
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        _eye.HandleRotation();
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= 5)
        {
            _eye.HandleAttack(gameTime);
            _eye.ChangeState(new CthulhuAggroState(_eye));
        }
    }
}