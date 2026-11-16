using Microsoft.Xna.Framework;
using terraria.StateMachines;

namespace terraria.Players.States;

internal sealed class JumpState : IState
{
    private readonly Player _player;

    public JumpState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("Jump");
        _player.Velocity = new Vector2(_player.Velocity.X, Player.JumpStrength);
        _player.IsGrounded = false;
    }

    public void Exit()
    {
    }

    public void Update(float deltaTime)
    {
        // Allow horizontal control while in the air.
        _player.HandleHorizontalMovement();

        if (_player.Velocity.Y > 0)
        {
            _player.ChangeState(Player.PlayerState.Fall);
            return;
        }
        PlayerMovementTransitions.LandIfGrounded(_player);
    }
}
