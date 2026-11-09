using System;
using terraria.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using terraria.Collision;

namespace terraria.Weapons;

public abstract class Weapon : IWeapon
{
    public ICollider Hitbox { get; protected set; }

    public AttackDefinition AttackData { get; }
    public int Damage => AttackData.Damage;
    public int AttackId { get; private set; }

    public Texture2D Texture { get; }

    public bool IsAttacking { get; protected set; }

    public float AttackDuration { get; protected set; }

    public float Rotation { get; protected set; }

    public float AttackTimer { get; set; }

    protected Weapon(
        Texture2D texture,
        float attackDuration = 0.25f, AttackDefinition attack = null)
    {
        Texture = texture;
        AttackData = attack ?? TerrariaStats.WoodenSword;
        AttackDuration = attackDuration;

        IsAttacking = false;
        AttackTimer = 0f;
    }

    public virtual void Attack(Vector2 playerLocation, bool facingRight)
    {
        if (IsAttacking)
        {
            return;
        }

        AttackId++;
        IsAttacking = true;
        AttackTimer = 0f;
    }

    public virtual void Attack(Vector2 playerLocation, bool facingRight, Point cursorPos)
        => Attack(playerLocation, facingRight);

    public virtual void Update(GameTime gameTime, bool facingRight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        if (!IsAttacking)
        {
            return;
        }
        AttackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (AttackTimer >= AttackDuration)
        {
            FinishAttack();
        }
    }

    protected virtual void FinishAttack()
    {
        IsAttacking = false;
        AttackTimer = 0f;
    }

    public abstract void Draw(
        SpriteBatch spriteBatch,
        Vector2 playerLocation,
        bool facingRight);

    public virtual void UpdateHitbox(Vector2 playerLocation, bool facingRight)
    {
    }

    protected static Vector2 GetHandPosition(Vector2 playerPosition, bool facingRight)
    {
        return playerPosition + new Vector2(facingRight ? 22 : 5, 24);
    }
}
