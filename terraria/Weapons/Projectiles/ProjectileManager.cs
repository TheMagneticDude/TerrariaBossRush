using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.Projectiles;

public class ProjectileManager
{
    private readonly List<IProjectile> _projectiles = new();

    public IReadOnlyList<IProjectile> Projectiles => _projectiles;

    public void RemoveInactive() => _projectiles.RemoveAll(projectile => !projectile.IsActive);

    public void AddProjectile(IProjectile projectile)
    {
        if (projectile == null)
        {
            throw new ArgumentNullException(nameof(projectile));
        }
        _projectiles.Add(projectile);
    }

    public void Update(GameTime gameTime)
    {
        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            _projectiles[i].Update(gameTime);
            if (!_projectiles[i].IsActive)
            {
                _projectiles.RemoveAt(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (IProjectile projectile in _projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }

    public void Clear()
    {
        _projectiles.Clear();
    }
}