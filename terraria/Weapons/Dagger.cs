using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Projectiles;

namespace terraria.Weapons;

public sealed class Dagger : ProjectileWeapon
{
    protected override float SpriteAngleOffset => MathHelper.PiOver2;
    protected override Vector2 GripOrigin => new Vector2(Texture.Width / 2f, Texture.Height - 4);

    public Dagger(Texture2D texture, ProjectileManager projectiles)
        : base(texture, texture, projectiles, TerrariaStats.MagicDagger,
            attackDuration: 0.25f, projectileSpeed: 500f, spawnOffset: 18f)
    {
    }

    protected override IProjectile CreateProjectile(Vector2 position, Vector2 velocity)
    {
        return new DaggerProjectile(ProjectileTexture, position, velocity, AttackData);
    }
}
