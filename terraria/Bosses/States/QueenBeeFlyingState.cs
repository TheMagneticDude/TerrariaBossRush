using System;

using Microsoft.Xna.Framework;

namespace terraria.Bosses.States;

public class QueenBeeFlyingState: IBossState
{
    private QueenBee _queenBee;
    private float _timer;

    public QueenBeeFlyingState(QueenBee queenBee)
    {
        _queenBee = queenBee;
    }

    public void Enter()
    {
        _timer = 0;
        _queenBee.SetAnimation("queen-bee-attack");
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _timer += elapsed;

        //timer to test states
        //_queenBee.Position += new Vector2(-100 * elapsed, 0);
        _queenBee.RunToPosition(_queenBee.TargetPos);//target player

        if (_timer >= 1)
        {
            _queenBee.ChangeState(new QueenBeeStingerState(_queenBee));
        }
    }

}