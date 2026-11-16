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
    private const int _border = 2;

    private readonly Color _normalSlot = new Color(55, 85, 115, 180);
    private readonly Color _selectedSlot = new Color(105, 140, 170, 220);
    private readonly Color _slotBorder = new Color(120, 150, 175, 200);

    public HotbarUI()
    {
    }

    public void Draw(SpriteBatch spriteBatch, PlayerHotbar hotBar)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(hotBar);

        if (_background == null)
        {
            _background = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);

            _background.SetData(new[] { Color.White });
        }

        int start = 10;
        int y = 10;

        for (int ii = 0; ii < hotBar.Count; ii++)
        {
            
            Rectangle slot = new Rectangle(start + ii * (_slotSize + _spacing), y, _slotSize, _slotSize);

            bool isSelected = ii == hotBar.SelectedSlot;

            Color slotColor = isSelected ? _selectedSlot : _normalSlot;

            spriteBatch.Draw(_background, slot, slotColor);

            IHotbarEntry entry = hotBar.GetEntry(ii);
            entry.DrawIcon(spriteBatch, slot);

            DrawBorder(spriteBatch, slot, _slotBorder);

        }
    }

    private void DrawBorder(SpriteBatch spriteBatch, Rectangle slot, Color color)
    {
        spriteBatch.Draw(_background,
            new Rectangle(slot.X, slot.Y, slot.Width, _border), color);

        spriteBatch.Draw(_background,
            new Rectangle(slot.X, slot.Bottom - _border, slot.Width, _border), color);

        spriteBatch.Draw(_background,
            new Rectangle(slot.X, slot.Y, _border, slot.Height), color);

        spriteBatch.Draw(_background,
            new Rectangle(slot.Right - _border, slot.Y, _border, slot.Height), color);
    }
}