using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Weapons;
using terraria.Collision;

public class Sword : Weapon
{
    private const float WeaponScale = 1.3f;
    private const float SwingStart = -MathHelper.PiOver4;
    private const float SwingEnd = 3f * MathHelper.PiOver4;

    public Sword(Texture2D texture) : base(texture, 0.25f)
    {
        ArgumentNullException.ThrowIfNull(texture);

        Hitbox = new RotatedRectangleCollider(
            Vector2.Zero, texture.Width * WeaponScale * 0.6f, texture.Height * WeaponScale * 0.75f);
    }

    public override void Update(GameTime gameTime, bool facingRight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        if (IsAttacking)
        {
            float elapsed = AttackTimer + (float)gameTime.ElapsedGameTime.TotalSeconds;
            float progress = MathHelper.Clamp(elapsed / AttackDuration, 0f, 1f);
            Rotation = MathHelper.Lerp(SwingStart, SwingEnd, progress) * (facingRight ? 1 : -1);
        }
        base.Update(gameTime, facingRight);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerLocation, bool facingRight)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        if (IsAttacking)
        {
            Vector2 weaponPosition = GetHandPosition(playerLocation, facingRight);

            Vector2 originOfWeapon = facingRight ? new Vector2(2f, Texture.Height - 2f) : new Vector2(Texture.Width - 2f, Texture.Height - 2f);

            SpriteEffects effect = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(Texture, weaponPosition, null, Color.White, Rotation, originOfWeapon, WeaponScale, effect, 0f);
            Hitbox.DrawDebug(spriteBatch);
        }
    }

    public override void UpdateHitbox(Vector2 playerLocation, bool facingRight)
    {
        if (Hitbox is not RotatedRectangleCollider collider)
        {
            return;
        }

        Vector2 weaponPosition = GetHandPosition(playerLocation, facingRight);

        Vector2 origin = facingRight
            ? new Vector2(2f, Texture.Height - 2f)
            : new Vector2(Texture.Width - 2f, Texture.Height - 2f);

        // The artwork is diagonal: follow the blade from the guard to its tip.
        Vector2 bladeStart = new Vector2(Texture.Width * 0.25f, Texture.Height * 0.75f);
        Vector2 bladeEnd = new Vector2(Texture.Width * 0.94f, Texture.Height * 0.06f);
        if (!facingRight)
        {
            bladeStart.X = Texture.Width - bladeStart.X;
            bladeEnd.X = Texture.Width - bladeEnd.X;
        }
        Vector2 bladeCenter = (bladeStart + bladeEnd) / 2f;
        Vector2 bladeDirection = bladeEnd - bladeStart;

        Vector2 localOffset = (bladeCenter - origin) * WeaponScale;
        float cos = MathF.Cos(Rotation);
        float sin = MathF.Sin(Rotation);

        Vector2 rotatedOffset = new Vector2(
            localOffset.X * cos - localOffset.Y * sin,
            localOffset.X * sin + localOffset.Y * cos);

        collider.Position = weaponPosition + rotatedOffset;
        collider.Width = bladeDirection.Length() * WeaponScale;
        collider.Height = Texture.Width * 0.22f * WeaponScale;
        collider.Rotation = Rotation + MathF.Atan2(bladeDirection.Y, bladeDirection.X);
    }
}
