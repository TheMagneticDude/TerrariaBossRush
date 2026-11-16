using System;
using Microsoft.Xna.Framework;

namespace terraria.Bosses.States;

public sealed class QueenBeeStingerState : IBossState
{
    // Tunable arena-scale timings; this project currently implements Classic only.
    private const float Duration = 5f;
    private const float ShotInterval = 0.4f;
    private const float FirstShotDelay = 0.6f;
    private const float HoverHeight = 200f;
    private const float MaximumSpeed = 7f;
    private readonly QueenBee _queenBee;
    private float _elapsed;
    private float _shotTimer;

    public QueenBeeStingerState(QueenBee queenBee)
    {
        _queenBee = queenBee ?? throw new ArgumentNullException(nameof(queenBee));
    }

    public void Enter()
    {
        _elapsed = 0f;
        _shotTimer = FirstShotDelay;
        _queenBee.Velocity = Vector2.Zero;
        _queenBee.Acceleration = Vector2.Zero;
        _queenBee.SetAnimation("queen-bee-attack");
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _elapsed += seconds;
        if (_elapsed >= Duration)
        {
            _queenBee.Acceleration = Vector2.Zero;
            _queenBee.Velocity = Vector2.Zero;
            _queenBee.ChangeState(new QueenBeeIdleState(_queenBee));
            return;
        }

        MoveAbovePlayer();
        _shotTimer -= seconds;
        if (_shotTimer <= 0f)
        {
            // Shoot from above; do not fire through the player while repositioning.
            if (_queenBee.Position.Y < _queenBee.TargetCenter.Y - 40f)
            {
                _queenBee.FireStinger();
            }
            // At most one shot per update avoids a burst after a long frame.
            _shotTimer = ShotInterval;
        }
    }

    private void MoveAbovePlayer()
    {
        float horizontalOffset = MathF.Sin(_elapsed * 1.5f) * 100f;
        Vector2 hoverPosition = _queenBee.TargetCenter + new Vector2(horizontalOffset, -HoverHeight);
        Vector2 velocity = (hoverPosition - _queenBee.Position) * 0.035f;
        if (velocity.LengthSquared() > MaximumSpeed * MaximumSpeed)
        {
            velocity = Vector2.Normalize(velocity) * MaximumSpeed;
        }
        // Boss movement uses pixels per tick; RoomManager applies this velocity.
        _queenBee.Acceleration = Vector2.Zero;
        _queenBee.Velocity = velocity;
    }
}
