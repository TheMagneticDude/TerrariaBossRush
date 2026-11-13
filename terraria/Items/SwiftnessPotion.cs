using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Players;

namespace terraria.Items;

public class SwiftnessPotion: IItem
{
    private const float DisplayDuration = 0.5f;
    private const float SwiftnessDuration = 10f;

    private Vector2 _potPosition;

    private readonly Texture2D _texture;

    private float _timer;

    public string Name
    {
        get { return "Swiftness Potion"; }
    }
    public bool IsActive { get; private set; }

    public SwiftnessPotion(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        _texture = texture;
        IsActive = false;
    }

    public void Use(IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);
        player.Swiftness(SwiftnessDuration);
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
