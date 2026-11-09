using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using terraria.Collision;

namespace terraria.Weapons;

public interface IWeapon
{
    ICollider Hitbox { get; }
    bool IsAttacking { get; }
    int Damage { get; }
    terraria.Combat.AttackDefinition AttackData { get; }
    int AttackId { get; }
    Texture2D Texture { get; }
    void Attack(Vector2 playerLocation, bool facingRight);
    void Attack(Vector2 playerLocation, bool facingRight, Point cursorPos);
    void Update(GameTime gameTime, bool facingRight);
    void Draw(SpriteBatch spriteBatch, Vector2 playerLocation, bool facingRight);

    void UpdateHitbox(Vector2 playerLocation, bool facingRight);
}
