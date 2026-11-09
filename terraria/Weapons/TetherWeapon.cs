using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Collision;
using terraria.Combat;

namespace terraria.Weapons;

public abstract class TetherWeapon : AimedWeapon
{
    protected Vector2 Tip;

    protected TetherWeapon(Texture2D texture, float duration, AttackDefinition attack)
        : base(texture, duration, attack)
    {
        Hitbox = new RotatedRectangleCollider(Vector2.Zero, 1, 1);
    }
}
