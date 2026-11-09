using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Collision;

namespace terraria.Projectiles;

public interface IProjectile
{
    ICollider Hitbox { get; }
    bool IsActive { get; }
    Vector2 Position { get; }
    int Damage { get; }
    AttackDefinition AttackData { get; }
    bool CanHitEnemy(HealthComponent target);
    void OnEnemyHit(HealthComponent target, bool damageApplied);
    void Destroy();
    
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}
