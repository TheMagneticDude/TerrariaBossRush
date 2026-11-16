using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using terraria.Bosses;
using terraria.Collision;
using terraria.Players;
using terraria.Projectiles;
using terraria.Weapons;

namespace terraria.Combat;

public sealed class CombatManager
{
    private readonly ProjectileManager _projectiles;
    private readonly HashSet<IBoss> _swingHits = new HashSet<IBoss>();
    private IWeapon _currentWeapon;
    private int _currentAttackId;

    public CombatManager(ProjectileManager projectiles)
    {
        _projectiles = projectiles ?? throw new ArgumentNullException(nameof(projectiles));
    }

    public void Update(GameTime gameTime, IPlayer player, IReadOnlyList<IBoss> bosses,
        IReadOnlyList<CollisionSurface> worldMesh)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(bosses);
        ArgumentNullException.ThrowIfNull(worldMesh);

        _projectiles.Update(gameTime);
        TrackAttack(player.EquippedWeapon);
        CheckPlayerCollisions(player, bosses);
        CheckProjectileCollisions(player, bosses, worldMesh);
        _projectiles.RemoveInactive();
    }

    private void TrackAttack(IWeapon weapon)
    {
        int attackId = weapon?.AttackId ?? 0;
        if (weapon == _currentWeapon && attackId == _currentAttackId)
        {
            return;
        }

        _swingHits.Clear();
        _currentWeapon = weapon;
        _currentAttackId = attackId;
    }

    private void CheckPlayerCollisions(IPlayer player, IReadOnlyList<IBoss> bosses)
    {
        if (player.IsDead)
        {
            return;
        }

        foreach (IBoss boss in bosses)
        {
            if (boss.Health.IsDead)
            {
                continue;
            }

            CheckMeleeHit(player.EquippedWeapon, boss);
            if (!boss.Health.IsDead && !player.IsDead && boss.Hitbox.Intersects(player.Hitbox))
            {
                player.TakeDamage(boss.ContactDamage);
            }

            if (player.IsDead)
            {
                break;
            }
        }
    }

    private void CheckMeleeHit(IWeapon weapon, IBoss boss)
    {
        if (weapon == null || !weapon.IsAttacking || weapon.Hitbox == null)
        {
            return;
        }

        if (_swingHits.Contains(boss) || !weapon.Hitbox.Intersects(boss.Hitbox))
        {
            return;
        }

        _swingHits.Add(boss);
        boss.Health.ReceiveHit(weapon.AttackData);
    }

    private void CheckProjectileCollisions(IPlayer player, IReadOnlyList<IBoss> bosses,
        IReadOnlyList<CollisionSurface> worldMesh)
    {
        foreach (IProjectile projectile in _projectiles.Projectiles)
        {
            if (!projectile.IsActive || projectile.Hitbox == null)
            {
                continue;
            }

            CheckTerrainHit(projectile, worldMesh);
            if (!projectile.IsActive)
            {
                continue;
            }

            if (projectile.Team == CombatTeam.Enemy)
            {
                CheckProjectilePlayerHit(projectile, player);
            }
            else if (projectile.Team == CombatTeam.Player)
            {
                CheckProjectileEnemyHits(projectile, bosses);
            }
        }
    }

    private static void CheckProjectilePlayerHit(IProjectile projectile, IPlayer player)
    {
        if (player.IsDead || !projectile.Hitbox.Intersects(player.Hitbox))
        {
            return;
        }

        // ReceiveHit applies damage and effects together, respecting invulnerability.
        player.ReceiveHit(projectile.AttackData);
        projectile.Destroy();
    }

    private static void CheckTerrainHit(IProjectile projectile, IReadOnlyList<CollisionSurface> worldMesh)
    {
        foreach (CollisionSurface surface in worldMesh)
        {
            if (projectile.Hitbox.Intersects(surface))
            {
                projectile.Destroy();
                return;
            }
        }
    }

    private static void CheckProjectileEnemyHits(IProjectile projectile, IReadOnlyList<IBoss> bosses)
    {
        foreach (IBoss boss in bosses)
        {
            if (boss.Health.IsDead || !projectile.CanHitEnemy(boss.Health)
                || !projectile.Hitbox.Intersects(boss.Hitbox))
            {
                continue;
            }

            bool damageApplied = boss.Health.ReceiveHit(projectile.AttackData);
            projectile.OnEnemyHit(boss.Health, damageApplied);
            if (!projectile.IsActive)
            {
                return;
            }
        }
    }

    public void Clear()
    {
        _projectiles.Clear();
        _swingHits.Clear();
        _currentWeapon = null;
        _currentAttackId = 0;
    }
}
