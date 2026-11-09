using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using terraria.Players;

namespace terraria.Items;

public class RecallPotion: IItem
{
    private const float DisplayDuration = 0.5f;
    private readonly Vector2 _recallLocation = new Vector2(200,450);

    private Vector2 _potPosition;

    private readonly Sprite _sprite;

    private float _timer;

    public string Name
    {
        get { return "Recall Potion"; }
    }
    public bool IsActive { get; private set; }

    public RecallPotion(TextureAtlas atlas)
    {
        ArgumentNullException.ThrowIfNull(atlas);
        _sprite = atlas.CreateSprite("recallPotion");
        _sprite.CenterOrigin();
        IsActive = false;
    }

    public void Use(IPlayer player)
    {
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
        if (IsActive)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer <= 0)
            {
                player.Position = _recallLocation;
                player.Velocity = Vector2.Zero;

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
            _sprite.Draw(spriteBatch, _potPosition);
        }
    }

    public void DrawIcon(SpriteBatch spriteBatch, Rectangle slot)
    {       
        _sprite.Draw(spriteBatch, new Vector2(slot.Center.X, slot.Center.Y));
    }
}

