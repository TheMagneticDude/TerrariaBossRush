using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Collision;
using terraria.Combat;

namespace terraria.Weapons;

public sealed class Yoyo : TetherWeapon
{
    private readonly Texture2D _head;
    private readonly Texture2D _stringPixel;

    public Yoyo(Texture2D texture, Texture2D head, Texture2D stringPixel) : base(texture, 1.1f, TerrariaStats.WoodenYoyo)
    {
        _head = head;
        _stringPixel = stringPixel;
    }

    public override void UpdateHitbox(Vector2 position, bool facingRight)
    {
        base.UpdateHitbox(position, facingRight);
        float progress = MathHelper.Clamp(AttackTimer / AttackDuration, 0, 1);
        float extension = MathHelper.Clamp(MathF.Min(progress / 0.25f, (1 - progress) / 0.3f), 0, 1);
        Vector2 normal = new Vector2(-Direction.Y, Direction.X);
        Tip = Hand + Direction * (180 * extension)
            + normal * (MathF.Sin(progress * MathHelper.TwoPi * 3) * 16 * extension);
        var collider = (RotatedRectangleCollider)Hitbox;
        collider.Position = Tip;
        collider.Width = collider.Height = 20;
    }

    public override void Draw(SpriteBatch batch, Vector2 position, bool facingRight)
    {
        if (!IsAttacking)
        {
            return;
        }
        ArgumentNullException.ThrowIfNull(batch);
        DrawString(batch);
        DrawHead(batch);
    }

    private void DrawString(SpriteBatch batch)
    {
        Vector2 delta = Tip - Hand;
        batch.Draw(_stringPixel, Hand, null, Color.LightGray,
            MathF.Atan2(delta.Y, delta.X), new Vector2(0, 0.5f),
            new Vector2(delta.Length(), 1), SpriteEffects.None, 0);
    }

    private void DrawHead(SpriteBatch batch)
    {
        batch.Draw(_head, Tip, null, Color.White, AttackTimer * 18,
            new Vector2(_head.Width / 2f, _head.Height / 2f), 1.25f, SpriteEffects.None, 0);
    }
}
