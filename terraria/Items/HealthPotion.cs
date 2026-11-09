using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Players;

namespace terraria.Items;

public class HealthPotion : IItem
{
    private const float DisplayDuration = 0.5f;

    private readonly Sprite _sprite;
    private float _timer;

    private Vector2 _potPosition;

    public string Name
    {
        get { return "Health Potion"; }
    }
    public bool IsActive { get; private set; }

   
    public HealthPotion(TextureAtlas atlas)
    {
        ArgumentNullException.ThrowIfNull(atlas);
        _sprite = atlas.CreateSprite("healthPotion");
        _sprite.CenterOrigin();
        IsActive = false;
    }

    public void Use(IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);
        
        player.Heal(30);

        IsActive = true;
        _timer = DisplayDuration;
    }

    public void Select(IPlayer player)
    {
    }

    public void Update(GameTime gameTime, IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        ArgumentNullException.ThrowIfNull(player);
        if (!IsActive)
        {
            return;
        }

        _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer <= 0f)
        {
            IsActive = false;
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
            _potPosition = player.Location + new Vector2(26, 22);
        }
        else
        {
            _potPosition = player.Location + new Vector2(-2, 22);
        }
        _sprite.Draw(spriteBatch, _potPosition);
    }

    public void DrawIcon(SpriteBatch spriteBatch, Rectangle slot)
    {
        _sprite.Draw(spriteBatch, new Vector2(slot.Center.X, slot.Center.Y));
    }
}
