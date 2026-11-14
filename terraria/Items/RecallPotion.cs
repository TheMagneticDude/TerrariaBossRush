using System;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Players;

namespace terraria.Items;

public class RecallPotion: IItem
{
    private const float DisplayDuration = 0.5f;
    private const float CooldownDuration = 10f;

    private readonly Vector2 _recallLocation = new Vector2(200,450);

    private Vector2 _potPosition;

    private readonly Texture2D _texture;

    private float _displayTimer;
    private float _cooldownTimer;

    public string Name
    {
        get { return "Recall Potion"; }
    }
    public bool IsActive { get; private set; }

    public bool IsOnCooldown
    {
        get { return _cooldownTimer > 0f; }
    }


    public RecallPotion(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        _texture = texture;
        IsActive = false;
    }

    public void Use(IPlayer player)
    {
        if (IsOnCooldown)
        {
            return;
        }

        IsActive = true;
        _displayTimer = DisplayDuration;
        _cooldownTimer = CooldownDuration;
    }

    public void Select(IPlayer player)
    {
    }

    public void Update(GameTime gameTime, IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        ArgumentNullException.ThrowIfNull(player);

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsActive)
        {
            _displayTimer -= elapsed;

            if (_displayTimer <= 0)
            {
                player.Position = _recallLocation;
                player.Velocity = Vector2.Zero;

                IsActive = false;
            }

        }
        if (IsOnCooldown)
        {
            _cooldownTimer -= elapsed;

            if(_displayTimer <= 0f)
            {
                IsActive = false;
            }
        }
    }
    public void Draw(SpriteBatch spriteBatch, IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(player);
        if(IsActive)
        {
            if (player.FacingRight)
            {
                _potPosition = player.Position + new Vector2(25, 22);
            }
            else
            {
                _potPosition = player.Position + new Vector2(-1, 22);
            }
            spriteBatch.Draw(
                _texture,
                _potPosition,
                null,
                Color.White,
                0f,
                new Vector2(_texture.Width / 2f, _texture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }

    public void DrawIcon(SpriteBatch spriteBatch, Rectangle slot)
    {       
        ArgumentNullException.ThrowIfNull(spriteBatch);
        
        spriteBatch.Draw(
            _texture,
            new Vector2(slot.Center.X, slot.Center.Y),
            null,
            Color.White,
            0f,
            new Vector2(_texture.Width / 2f, _texture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}

