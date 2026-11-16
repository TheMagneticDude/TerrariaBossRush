using terraria.StateMachines;

namespace terraria.Players.States;

internal sealed class IdleState : IState
{
    private readonly Player _player;

    public IdleState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("Idle");
    }

    public void Exit()
    {
    }

    public void Update(float deltaTime)
    {
        _player.HandleHorizontalMovement();

        PlayerMovementTransitions.UpdateGroundedState(_player);
    }
}
