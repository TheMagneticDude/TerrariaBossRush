using terraria.StateMachines;

namespace terraria.Players.States;

internal sealed class FallState : IState
{
    private readonly Player _player;

    public FallState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("Fall");
    }

    public void Exit()
    {
    }

    public void Update(float deltaTime)
    {
        // Allow horizontal control while in the air.
        _player.HandleHorizontalMovement();

        PlayerMovementTransitions.LandIfGrounded(_player);
    }
}
