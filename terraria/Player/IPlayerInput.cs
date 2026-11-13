using Microsoft.Xna.Framework;

namespace terraria.Players;

// Gameplay consumes controls without knowing whether they came from a keyboard
// or a remote player. Updating the source remains the game loop's responsibility.
public interface IPlayerInput
{
    bool IsPressed(KeyAction action);
    bool IsNewPress(KeyAction action);
    Point GetCursorPos();
}
