using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using terraria.Hotbar;

namespace terraria.UI;

public class HotbarUI
{
    private Texture2D _background;
    private int _slotSize = 50;
    private int _spacing = 4;

    public HotbarUI()
    {
    }

    public void Draw(SpriteBatch spriteBatch, PlayerHotbar hotBar)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(hotBar);

        if (_background == null)
        {
            _background = new Texture2D(
                spriteBatch.GraphicsDevice,
                1,
                1);

            _background.SetData(new[] { Color.White });
        }

        int start = 10;
        int y = 10;

        for (int ii = 0; ii < hotBar.Count; ii++)
        {
            
            Rectangle slot = new Rectangle(start + ii * (_slotSize + _spacing), y, _slotSize, _slotSize);

            spriteBatch.Draw(_background, slot, Color.Gray);

            if (ii == hotBar.SelectedSlot)
            {
                spriteBatch.Draw(_background, slot, Color.White * 0.4f);
            }

            IHotbarEntry entry = hotBar.GetEntry(ii);
            entry.DrawIcon(spriteBatch, slot);

        }
    }
}