using MonoGameLibrary.Input;

namespace terraria.Players.States;

internal static class PlayerMovementTransitions
{
    public static void UpdateGroundedState(Player player)
    {
        if (!player.IsGrounded)
        {
            player.ChangeState(Player.PlayerState.Fall);
        }
        else if (player.Input.IsPressed(KeyAction.MoveUp))
        {
            player.ChangeState(Player.PlayerState.Jump);
        }
        else
        {
            ChangeToGroundMovement(player);
        }
    }

    public static void LandIfGrounded(Player player)
    {
        if (player.IsGrounded)
        {
            ChangeToGroundMovement(player);
        }
    }

    private static void ChangeToGroundMovement(Player player)
    {
        player.ChangeState(player.Velocity.X != 0 ? Player.PlayerState.Walk : Player.PlayerState.Idle);
    }
}
