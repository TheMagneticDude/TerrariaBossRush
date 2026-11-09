using terraria.Collision;
using terraria.Combat;
using terraria.UI;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Bosses.States;
using terraria.Entity;
using static terraria.Constants;
//temp
using MonoGameLibrary.Input;
namespace terraria.Bosses;


public class QueenBee : IBoss
{
    public HealthComponent Health { get; } = new(1200, 0f, invulnerabilityDuration: 0f);
    public int ContactDamage => 20;
    private readonly RotatedRectangleCollider _hitbox = new(Vector2.Zero, 1f, 1f);

    public Vector2 Position { get; set; }
    public Vector2 Velocity {get; set;}
    public Vector2 Acceleration {get; set;}
    public Vector2 JerkDamping {get; set;}//drag force on acceleration
    public Vector2 Drag {get; set;}
    public IEntity Target { get; set; }
    public Vector2 TargetPos {get;set;}
    public ICollider Hitbox
    {
        get
        {
            // Position is the shared center for rendering and collision resolution.
            _hitbox.Width = _sprite.Width;
            _hitbox.Height = _sprite.Height;
            _hitbox.Rotation = 0f;
            _hitbox.Update(Position);
            return _hitbox;
        }
    }
    public bool IsGrounded { get; set; }

    private AnimatedSprite _sprite;

    private TextureAtlas _atlas;

    private IBossState _state;

    private bool _facingRight = true;

    public void ChangeState(IBossState newState)
    {
        _state = newState;
        _state.Enter();
    }

    public QueenBee(AnimatedSprite sprite, TextureAtlas atlas, IEntity target = null)
    {
        _sprite = sprite;
        _atlas = atlas;
        Position = new Vector2(1100, 400);
        Velocity = new Vector2(0, 0); 
        Drag = new Vector2(0.02f,0.02f);//constantly subtract or add from velocity towards 0 (acceleration vector acting against direction of movement)
        JerkDamping = new Vector2(0f,0f);
        Target = target;
        ChangeState(new QueenBeeIdleState(this));

        _sprite.CenterOrigin();
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


    private float CalcDist(float target, float currPos)
    {
        float epsilon = 0.03f;
        float dist = target - currPos;

        if (Math.Abs(dist) >= epsilon)
        {
            return dist;
        }
        return 0;
    }
    public void RunToPosition(Vector2 TargetPosition)
    {
        Vector2 direction = TargetPosition - Position;
        float epsilon = 0.03f;
        if (direction.Length() > epsilon)
        {
            direction.Normalize();

            float accelerationForce = 0.5f; 
            Acceleration = direction * accelerationForce;
        }
        else
        {
            Acceleration = Vector2.Zero;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Health.IsDead) return;
        _sprite.CenterOrigin();
        SpriteEffects effect = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        HitFlash.Draw(spriteBatch, _sprite, Position, Health.IsHitFlashing, _sprite.Color, effect);
        Hitbox.DrawDebug(spriteBatch);
    }
}
