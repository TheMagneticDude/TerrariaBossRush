using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;

namespace terraria.Projectiles;

public sealed class MagicBoltProjectile : Projectile
{
    public MagicBoltProjectile(Texture2D texture, Vector2 position, Vector2 velocity,
        AttackDefinition attack = null)
        : base(texture, position, velocity, attack ?? TerrariaStats.AmethystStaff)
    {
    }

    public override void Draw(SpriteBatch batch) => DrawSprite(batch, 0.5f);
}
