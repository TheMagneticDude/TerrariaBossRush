using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Projectiles;

namespace terraria.Weapons;

public sealed class Bow : ProjectileWeapon
{
    public Bow(Texture2D texture,
        Texture2D projectileTexture,
        ProjectileManager projectiles,
        AttackDefinition attack = null)
        : base(texture, projectileTexture, projectiles, attack ?? TerrariaStats.WoodenBow,
            attackDuration: 0.4f, projectileSpeed: 650f, spawnOffset: 18f)
    {
    }

    protected override IProjectile CreateProjectile(Vector2 position, Vector2 velocity)
    {
        return new ArrowProjectile(ProjectileTexture, position, velocity, AttackData);
    }
}
