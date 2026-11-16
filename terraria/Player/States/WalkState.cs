using terraria.StateMachines;

namespace terraria.Players.States;

internal sealed class WalkState : IState
{
    private readonly Player _player;

    public WalkState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("Walk");
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
