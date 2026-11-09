using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Collision;
using terraria.Combat;

namespace terraria.Weapons;

public sealed class Whip : TetherWeapon
{
    private const int SegmentCount = 12;
    private readonly Texture2D _segments;

    public Whip(Texture2D texture, Texture2D segments) : base(texture, 0.45f, TerrariaStats.LeatherWhip)
    {
        _segments = segments;
    }

    public override void UpdateHitbox(Vector2 position, bool facingRight)
    {
        base.UpdateHitbox(position, facingRight);
        float progress = MathHelper.Clamp(AttackTimer / AttackDuration, 0, 1);
        float swing = MathHelper.Lerp(-0.9f, 0.9f, progress) * (facingRight ? 1 : -1);
        float angle = MathF.Atan2(Direction.Y, Direction.X) + swing;
        float length = 20 + 190 * MathF.Sin(progress * MathHelper.Pi);
        Tip = Hand + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * length;
        var collider = (RotatedRectangleCollider)Hitbox;
        collider.Position = (Hand + Tip) / 2;
        collider.Width = length;
        collider.Height = 9;
        collider.Rotation = angle;
    }

    public override void Draw(SpriteBatch batch, Vector2 position, bool facingRight)
    {
        if (!IsAttacking)
        {
            return;
        }
        ArgumentNullException.ThrowIfNull(batch);
        DrawSegments(batch);
    }

    private void DrawSegments(SpriteBatch batch)
    {
        Vector2 delta = Tip - Hand;
        float rotation = MathF.Atan2(delta.Y, delta.X) - MathHelper.PiOver2;
        for (int i = 0; i < SegmentCount; i++)
        {
            DrawSegment(batch, i, delta, rotation);
        }
    }

    private void DrawSegment(SpriteBatch batch, int index, Vector2 delta, float rotation)
    {
        Rectangle source = SegmentSource(index);
        Vector2 position = Hand + delta * (index / (float)SegmentCount);
        Vector2 scale = new Vector2(1, delta.Length() / SegmentCount / source.Height);
        batch.Draw(_segments, position, source, Color.White, rotation,
            new Vector2(source.Width / 2f, 0), scale, SpriteEffects.None, 0);
    }

    private static Rectangle SegmentSource(int index)
    {
        if (index == 0)
        {
            return new Rectangle(0, 0, 10, 26);
        }
        if (index == SegmentCount - 1)
        {
            return new Rectangle(0, 120, 10, 18);
        }
        return new Rectangle(0, 36 + ((index - 1) % 3) * 28, 10, 16);
    }
}
