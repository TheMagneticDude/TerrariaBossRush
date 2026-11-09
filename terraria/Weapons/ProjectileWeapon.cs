using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Projectiles;

namespace terraria.Weapons;

public abstract class ProjectileWeapon : AimedWeapon
{
    private readonly ProjectileManager _projectiles;
    private readonly float _projectileSpeed;
    private readonly float _spawnOffset;

    protected Texture2D ProjectileTexture { get; }

    protected ProjectileWeapon(
        Texture2D texture,
        Texture2D projectileTexture,
        ProjectileManager projectiles,
        AttackDefinition attack,
        float attackDuration,
        float projectileSpeed,
        float spawnOffset)
        : base(texture, attackDuration, attack)
    {
        _projectiles = projectiles ?? throw new ArgumentNullException(nameof(projectiles));
        ProjectileTexture = projectileTexture;
        _projectileSpeed = projectileSpeed;
        _spawnOffset = spawnOffset;
    }

    protected override void BeginAttack()
    {
        Vector2 position = Hand + Direction * _spawnOffset;
        Vector2 velocity = Direction * _projectileSpeed;
        IProjectile projectile = CreateProjectile(position, velocity);
        _projectiles.AddProjectile(projectile);
    }

    protected abstract IProjectile CreateProjectile(Vector2 position, Vector2 velocity);
}
