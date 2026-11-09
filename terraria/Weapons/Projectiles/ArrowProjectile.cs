using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;

namespace terraria.Projectiles;

public sealed class ArrowProjectile : Projectile
{
    public ArrowProjectile(Texture2D texture, Vector2 position, Vector2 velocity,
        AttackDefinition attack = null)
        : base(texture, position, velocity, attack ?? TerrariaStats.WoodenBow, 360)
    {
    }

    public override void Draw(SpriteBatch batch) => DrawSprite(batch, 0.75f);
}
