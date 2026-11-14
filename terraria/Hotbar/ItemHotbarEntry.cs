using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Players;
using terraria.Items;

namespace terraria.Hotbar;

public class ItemHotbarEntry : IHotbarEntry
{
     private readonly IItem _item;

     private Texture2D _cooldownOverlay;

    public string Name { get; }
    
    public ItemHotbarEntry(string name, IItem item)
    {
        Name = name;
        _item = item;
    }

    public void Use(IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);

        _item.Use(player);
    }

    public void Select(IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);

        player.UnequipWeapon();
    }

    public void Update(GameTime gameTime, IPlayer player)
    {
        _item.Update(gameTime, player);
    }

    public void Draw(SpriteBatch spriteBatch, IPlayer player)
    {
        _item.Draw(spriteBatch, player);
    }

    public void DrawIcon(SpriteBatch spriteBatch, Rectangle slot)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _item.DrawIcon(spriteBatch, slot);

        if (_item.IsOnCooldown)
        {
            if (_cooldownOverlay == null)
            {
                _cooldownOverlay = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);

                _cooldownOverlay.SetData(new[] { Color.White });
            }

            spriteBatch.Draw(_cooldownOverlay, slot, new Color(0, 0, 0, 90));
        }
    }

}
