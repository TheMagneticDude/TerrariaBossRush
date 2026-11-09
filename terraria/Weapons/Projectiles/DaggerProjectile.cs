using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Collision;

namespace terraria.Projectiles;

public sealed class DaggerProjectile : Projectile
{
    public const float StraightFlightDistance = 24f * 16f;
    private const float SpinSpeed = 24f;
    private const float FallGravity = 1440f;
    private const float MaxFallSpeed = 960f;
    private const float HorizontalDrag = 1.827552f;
    private const float EnemyHitCooldown = 20f / 60f;
    private readonly Dictionary<HealthComponent, float> _nextEnemyHit = new();
    private readonly float _initialRotation;
    private readonly float _spinDirection;
    private float _straightDistance;
    private float _spinTime;
    private int _hitsRemaining = 3;
    public bool IsFalling { get; private set; }
    public float Rotation => _initialRotation + _spinDirection * SpinSpeed * _spinTime;
    protected override float SpriteRotation => Rotation;

    protected override float ColliderRotation => Rotation;

    public DaggerProjectile(Texture2D texture, Vector2 position, Vector2 velocity,
        AttackDefinition attack = null)
        : base(texture, position, velocity, attack ?? TerrariaStats.MagicDagger,
            width: 12, height: 28)
    {
        _initialRotation = MathF.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver2;
        _spinDirection = velocity.X < 0 ? -1f : 1f;
        ((RotatedRectangleCollider)Hitbox).Rotation = Rotation;
    }

    protected override void Move(float seconds)
    {
        float fallingTime = IsFalling ? seconds : MoveStraight(seconds);
        if (!IsFalling)
        {
            return;
        }
        _spinTime += fallingTime;
        MoveFalling(fallingTime);
    }

    private float MoveStraight(float seconds)
    {
        float speed = Velocity.Length();
        float distanceLeft = MathF.Max(0, StraightFlightDistance - _straightDistance);
        float straightTime = speed > 0 ? MathF.Min(seconds, distanceLeft / speed) : 0;
        Position += Velocity * straightTime;
        _straightDistance += speed * straightTime;
        IsFalling = speed == 0 || _straightDistance >= StraightFlightDistance - 0.001f;
        return seconds - straightTime;
    }

    private void MoveFalling(float seconds)
    {
        float drag = MathF.Exp(-HorizontalDrag * seconds);
        float horizontalDistance = Velocity.X * (1 - drag) / HorizontalDrag;
        float verticalSpeed = MathF.Min(Velocity.Y, MaxFallSpeed);
        Position += new Vector2(horizontalDistance, FallingDistance(verticalSpeed, seconds));
        Velocity = new Vector2(Velocity.X * drag,
            MathF.Min(MaxFallSpeed, verticalSpeed + FallGravity * seconds));
    }

    private static float FallingDistance(float initialSpeed, float seconds)
    {
        float acceleratingTime = MathF.Min(seconds, (MaxFallSpeed - initialSpeed) / FallGravity);
        float distance = initialSpeed * acceleratingTime
            + 0.5f * FallGravity * acceleratingTime * acceleratingTime;
        return distance + MaxFallSpeed * (seconds - acceleratingTime);
    }

    public override bool CanHitEnemy(HealthComponent target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return IsActive && (!_nextEnemyHit.TryGetValue(target, out float nextHitTime) || Age >= nextHitTime);
    }

    public override void OnEnemyHit(HealthComponent target, bool damageApplied)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (!damageApplied)
        {
            return;
        }
        _nextEnemyHit[target] = Age + EnemyHitCooldown;
        if (--_hitsRemaining <= 0)
        {
            Destroy();
        }
    }

    public override void Draw(SpriteBatch batch) => DrawSprite(batch, 1f);
}
