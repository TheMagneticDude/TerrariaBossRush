using terraria.Collision;
using terraria.Combat;
using terraria.UI;
using System.Reflection.Metadata;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Entity;
using static terraria.Constants;



namespace terraria.Bosses;

public class Slime: IBoss
{
    public HealthComponent Health { get; } = new(TerrariaStats.BlueSlimeHealth, invulnerabilityDuration: 0f);
    public int ContactDamage => TerrariaStats.BlueSlimeContact;
    private readonly RotatedRectangleCollider _hitbox = new(Vector2.Zero, 1f, 1f);

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Drag {get; set;}
    public bool IsGrounded { get;  set; }
    public Vector2 TargetPos {get;set;}
    public Color Color {get;set;}
    public IEntity Target { get; set; }

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

    private float _jumpStrength = -10f - (float)random.Next(10);
    //private bool jumped = false;
    private float _timer;

    private static Random random = new Random();
    private AnimatedSprite _sprite;

    private TextureAtlas _atlas;
    private uint _movementSpeed = 5 + (uint) random.Next(5);

    

    
    public Slime(AnimatedSprite sprite, TextureAtlas atlas, IEntity target = null)
    {
        _sprite = sprite;
        _atlas = atlas;
        Position = new Vector2(200 - random.Next(10)*10, 100); //randomize where they spawn
        Drag = new Vector2(2f,0f);
        Velocity = new Vector2(0, 0);
        Color = Color.White;
        Target = target;

        _sprite.CenterOrigin();
    }

    public Slime(AnimatedSprite sprite, TextureAtlas atlas, Color c, IEntity target = null)
    {
        _sprite = sprite;
        _atlas = atlas;
        Position = new Vector2(200 - random.Next(10)*10, 80);
        Drag = new Vector2(2f,0f);
        Velocity = new Vector2(0, 0);
        Color = c;
        Target = target;
        _sprite.CenterOrigin();
    }

    public void SetColor(Color c)
    {
        Color = c;
    }

    public void SetAnimation(string animationName)
    {
        _sprite.Animation = _atlas.GetAnimation(animationName);
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        if (Health.IsDead) return;
        Health.Update(gameTime);
        if (Health.IsDead) return;
        if (Target != null)
        {
            TargetPos = Target.Position;
        }
        _sprite.Update(gameTime);
        ApplyPhysics();
        HandleMovement();
        HandleJump(gameTime);
    }

    public void HandleMovement()
    {
        // RoomManager applies velocity and resolves world collisions.
        if (IsGrounded){Velocity = CalcDrag(Velocity, Drag);}
        
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

    private void ApplyPhysics()
    {
        Hitbox.Update(Position);
        Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity);
        //void floor
        // if (Position.Y >= GroundLevel)
        // {
        //     Position = new Vector2(Position.X, GroundLevel);
        //     Velocity = new Vector2(Velocity.X, 0f);
        //     IsGrounded = true;
        // }
        // else
        // {
        //     IsGrounded = false;
        // }

        if (IsGrounded)
        {
            Velocity = new Vector2(Velocity.X, 0f);
        }
    }
    private void HandleHorizontalMovement()
    {
        float componentX = (TargetPos.X - Position.X) >=  0 ? 1 : -1;

        Velocity = new Vector2(componentX * _movementSpeed, Velocity.Y);
    }

    private void HandleJump(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (IsGrounded)
        {
            if (_timer >= 2f)
            {
                HandleHorizontalMovement();
                Velocity = new Vector2(Velocity.X, _jumpStrength);
                IsGrounded = false;
                _timer = 0f;
            }
        }
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Health.IsDead) return;
        _sprite.CenterOrigin();
        HitFlash.Draw(spriteBatch, _sprite, Position, Health.IsHitFlashing, Health.Effects.Tint(Color));
        Hitbox.DrawDebug(spriteBatch);
    }
}
