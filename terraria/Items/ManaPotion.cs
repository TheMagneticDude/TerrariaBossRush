using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Players;

namespace terraria.Items;

public class ManaPotion: IItem
{
    private const float DisplayDuration = 0.5f;
    private const float CooldownDuration = 10f;

    private Vector2 _potPosition;

    private readonly Texture2D _texture;

    private float _displayTimer;
    private float _cooldownTimer;

    public string Name
    {
        get { return "Mana Potion"; }
    }
    public bool IsActive { get; private set; }

    public bool IsOnCooldown
    {
        get { return _cooldownTimer > 0f; }
    }

    public ManaPotion(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        _texture = texture;
        IsActive = false;
    }

    public void Use(IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (IsOnCooldown)
        {
            return;
        }
        player.RestoreMana(100);
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

            if (_displayTimer <= 0f)
            {
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
        
        if(!IsActive)
        {
            return;
        }
        
        if (player.FacingRight)
        {
            _potPosition = player.Position + new Vector2(26, 22);
        }
        else
        {
            _potPosition = player.Position + new Vector2(-2, 22);
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
