using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Collision;
using terraria.Combat;

namespace terraria.Projectiles;

public abstract class Projectile : IProjectile
{
    public ICollider Hitbox { get; protected set; }
    public AttackDefinition AttackData { get; }
    public int Damage => AttackData.Damage;
    public Texture2D Texture { get; }
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public bool IsActive { get; protected set; } = true;
    protected float Age { get; private set; }
    protected virtual float SpriteRotation => MathF.Atan2(Velocity.Y, Velocity.X) + MathHelper.PiOver2;
    protected virtual float ColliderRotation => MathF.Atan2(Velocity.Y, Velocity.X);
    private readonly float _gravity;
    protected Projectile(Texture2D texture, Vector2 position, Vector2 velocity,
        AttackDefinition attack, float gravity = 0, float width = 18, float height = 6)
    {
        Texture = texture;
        Position = position;
        Velocity = velocity;
        AttackData = attack ?? throw new ArgumentNullException(nameof(attack));
        _gravity = gravity;
        Hitbox = new RotatedRectangleCollider(position, width, height);
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        Hitbox.Position = Position;
        ((RotatedRectangleCollider)Hitbox).Rotation = ColliderRotation;
    }

    public virtual void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        if (!IsActive)
        {
            return;
        }
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Age += seconds;
        Move(seconds);
        UpdateCollider();
        if (Age >= 4f)
        {
            Destroy();
        }
    }

    protected virtual void Move(float seconds)
    {
        Velocity += new Vector2(0, _gravity * seconds);
        Position += Velocity * seconds;
    }

    public virtual bool CanHitEnemy(HealthComponent target) => IsActive;
    public virtual void OnEnemyHit(HealthComponent target, bool damageApplied) => Destroy();

    protected void DrawSprite(SpriteBatch batch, float scale)
    {
        ArgumentNullException.ThrowIfNull(batch);
        if (!IsActive)
        {
            return;
        }
        batch.Draw(Texture, Position, null, AttackData.Tint,
            SpriteRotation,
            new Vector2(Texture.Width / 2f, Texture.Height / 2f),
            scale, SpriteEffects.None, 0f);
    }

    public abstract void Draw(SpriteBatch spriteBatch);
    public virtual void Destroy() => IsActive = false;
}
