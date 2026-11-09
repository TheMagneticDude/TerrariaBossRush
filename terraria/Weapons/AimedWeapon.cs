using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.World;
using terraria.Combat;

namespace terraria.Weapons;

public abstract class AimedWeapon : Weapon
{
    protected Vector2 Direction = Vector2.UnitX;
    protected Vector2 Hand;

    protected virtual float SpriteAngleOffset => 0f;
    protected virtual Vector2 GripOrigin => new Vector2(Texture.Width / 2f, Texture.Height / 2f);
    protected AimedWeapon(Texture2D texture, float duration, AttackDefinition attack = null)
        : base(texture, duration, attack)
    {
    }

    public override void Attack(Vector2 position, bool facingRight) =>
        AttackToward(position, facingRight, new Vector2(facingRight ? 1 : -1, 0));

    public override void Attack(Vector2 position, bool facingRight, Point cursor)
    {
        Vector2 handPosition = GetHandPosition(position, facingRight);
        Vector2 targetPosition = WorldHandler.PixelToWorldSpace(cursor.ToVector2());
        AttackToward(position, facingRight, targetPosition - handPosition);
    }

    private void AttackToward(Vector2 position, bool facingRight, Vector2 direction)
    {
        if (IsAttacking)
        {
            return;
        }
        Direction = direction.LengthSquared() < 0.001f
            ? new Vector2(facingRight ? 1 : -1, 0) : Vector2.Normalize(direction);
        base.Attack(position, facingRight);
        UpdateHitbox(position, facingRight);
        BeginAttack();
    }

    protected virtual void BeginAttack()
    {
    }

    public override void UpdateHitbox(Vector2 position, bool facingRight)
    {
        Hand = GetHandPosition(position, facingRight);
        if (!IsAttacking)
        {
            Direction = facingRight ? Vector2.UnitX : -Vector2.UnitX;
        }
    }

    public override void Draw(SpriteBatch batch, Vector2 position, bool facingRight)
    {
        ArgumentNullException.ThrowIfNull(batch);
        if (!IsAttacking)
        {
            return;
        }

        bool flip = Direction.X < 0;
        float rotation = MathF.Atan2(Direction.Y, Direction.X)
            + (flip ? MathHelper.Pi - SpriteAngleOffset : SpriteAngleOffset);
        Vector2 origin = GripOrigin;
        if (flip)
        {
            origin.X = Texture.Width - origin.X;
        }

        batch.Draw(Texture, GetHandPosition(position, facingRight), null, AttackData.Tint, rotation, origin,
            1f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
    }
}
