using terraria.Collision;
using terraria.Combat;
using terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Entity;
using System;

using terraria.Bosses.States;

namespace terraria.Bosses;

public class EyeOfCthulhu: IBoss
{
    public HealthComponent Health { get; } = new(1000, 0f, invulnerabilityDuration: 0f);
    public int ContactDamage => 20;
    private readonly RotatedRectangleCollider _hitbox = new(Vector2.Zero, 1f, 1f);

    public Vector2 Position { get; set; }
    public Vector2 Velocity {get; set;}
    public Vector2 Acceleration {get; set;}
    public Vector2 JerkDamping {get; set;}//drag force on acceleration
    public Vector2 Drag {get; set;}

    public Vector2 TargetPos {get;set;}
    public float Rotation { get; set; }
    public IEntity Target { get; set; }
    public ICollider Hitbox
    {
        get
        {
            // Position is the shared center for rendering and collision resolution.
            _hitbox.Width = _sprite.Width;
            _hitbox.Height = _sprite.Height;
            _hitbox.Rotation = Rotation;
            _hitbox.Update(Position);
            return _hitbox;
        }
    }
    public bool IsGrounded { get; set; }
    private float targetRotation;

    private AnimatedSprite _sprite;

    private TextureAtlas _atlas;

    private IBossState _state;

    private uint _movementSpeed = 12;

    private bool _facingRight = true;

    public void ChangeState(IBossState newState)
    {
        _state = newState;
        _state.Enter();
    }

    public EyeOfCthulhu(AnimatedSprite sprite, TextureAtlas atlas, IEntity target = null)
    {
        _sprite = sprite;
        _atlas = atlas;
        Position = new Vector2(1100, 600);
        Velocity = new Vector2(0, 0); 
        Drag = new Vector2(0.2f,0.2f);
        JerkDamping = new Vector2(0.002f,0.002f);
        Target = target;

        _sprite.CenterOrigin();

        ChangeState(new CthulhuPhase1State(this));
    }

     public void SetAnimation(string animationName)
    {
        _sprite.Animation = _atlas.GetAnimation(animationName);
    }


    public void Update(GameTime gameTime)
    {
        if (Health.IsDead) return;
        Health.Update(gameTime);
        if (Target != null)
        {
            TargetPos = Target.Position;
        }
        _state.Update(gameTime);
        _sprite.Update(gameTime);
        
        
        HandleMovement();
    }

    public void HandleAttack(GameTime gameTime)
    {
            RunToPosition(TargetPos);
            DashToPosition(TargetPos);
    }

    public void HandleRotation()
    {
        Vector2 direction = TargetPos - Position;
        direction.Normalize();
        if (direction != Vector2.Zero)
        {
            targetRotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;

            float angleDifference = MathHelper.WrapAngle(targetRotation - Rotation);

            float turnSpeed = 0.05f;

            Rotation += angleDifference * turnSpeed;
            //wrap bounds
            Rotation = MathHelper.WrapAngle(Rotation);
        }
    }

    public void HandleMovement()
    {
        
        
        Velocity += Acceleration;
        // RoomManager applies velocity and resolves world collisions.
        
        

        Velocity = CalcDrag(Velocity, Drag);
        Acceleration = CalcDrag(Acceleration, JerkDamping);

        if (Velocity.X > 0) _facingRight = true;
        else if (Velocity.X < 0) _facingRight = false;

    }

    public Vector2 CalcDrag(Vector2 vel, Vector2 drag)
    {
        return new Vector2(HandleDragComponent(vel.X, drag.X), HandleDragComponent(vel.Y, drag.Y));
    }

    public float HandleDragComponent(float velComp, float dragComp)
    {
        if (Math.Abs(velComp) <= dragComp)
        {
            return 0;
        }

        if (velComp > 0)
        {
            velComp -= dragComp;
        }
        else
        {
            velComp += dragComp;
        }
        return velComp;
    }

    public void RunToPosition(Vector2 TargetPosition)
    {
        Vector2 direction = TargetPosition - Position;
        float epsilon = 0.03f;
        if (direction.Length() > epsilon)
        {
            direction.Normalize();

            float accelerationForce = 0.1f; 
            Acceleration = direction * accelerationForce;
        }
        else
        {
            Acceleration = Vector2.Zero;
        }
    }

    public void DashToPosition(Vector2 TargetPosition)
    {
        Vector2 direction = TargetPosition - Position;
        
        direction.Normalize(); 
        Velocity = direction * _movementSpeed;
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Health.IsDead) return;
        _sprite.CenterOrigin();
        SpriteEffects effect = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        HitFlash.Draw(spriteBatch, _sprite, Position, Health.IsHitFlashing, _sprite.Color, effect, Rotation);
        Hitbox.DrawDebug(spriteBatch);
    }
}
