using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Projectiles;

namespace terraria.Weapons;

public sealed class Staff : ProjectileWeapon
{
    protected override float SpriteAngleOffset => MathHelper.PiOver4;
    protected override Vector2 GripOrigin => new Vector2(4, Texture.Height - 4);

    public Staff(Texture2D texture,
        Texture2D projectileTexture,
        ProjectileManager projectiles,
        AttackDefinition attack = null)
        : base(texture, projectileTexture, projectiles, attack ?? TerrariaStats.AmethystStaff,
            attackDuration: 0.55f, projectileSpeed: 450f, spawnOffset: 22f)
    {
    }

    protected override IProjectile CreateProjectile(Vector2 position, Vector2 velocity)
    {
        return new MagicBoltProjectile(ProjectileTexture, position, velocity, AttackData);
    }
}
