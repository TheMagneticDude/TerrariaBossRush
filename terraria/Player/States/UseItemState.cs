using Microsoft.Xna.Framework;
using MonoGameLibrary.Input;
using terraria.StateMachines;

namespace terraria.Players.States;

internal sealed class UseItemState : IState
{
    private readonly Player _player;

    public UseItemState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("UseItem");
    }

    public void Exit()
    {
    }

    public void Update(float deltaTime)
    {
        //Move slower while attacking or using item
        float attackSpeedModifier = 0.9f;
        // Allow horizontal control
        _player.HandleHorizontalMovement();
        _player.Velocity = new Vector2(_player.Velocity.X * attackSpeedModifier, _player.Velocity.Y);

        if (_player.Velocity.X != 0)
        {
            _player.ChangeState(Player.PlayerState.Walk);
        }
        else
        {
            _player.ChangeState(Player.PlayerState.Idle);
        }
    }
}
