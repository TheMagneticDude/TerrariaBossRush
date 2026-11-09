using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Players;

namespace terraria.Hotbar;

public class PlayerHotbar
{
    private readonly IHotbarEntry[] _slots;

    public int Count
    {
        get { return _slots.Length; }
    }

    public int SelectedSlot { get; private set; }

    public IHotbarEntry SelectedEntry
    {
        get { return _slots[SelectedSlot]; }
    }

    public PlayerHotbar(IHotbarEntry[] slots)
    {
        if (slots == null || slots.Length == 0)
        {
            throw new ArgumentException("A hotbar needs at least one slot.", nameof(slots));
        }

        _slots = new IHotbarEntry[slots.Length];
        Array.Copy(slots, _slots, slots.Length);
        SelectedSlot = 0;
    }

    public IHotbarEntry GetEntry(int slot)
    {
        return _slots[slot];
    }

    public void UseSelected(IPlayer player)
    {
        SelectedEntry.Use(player);
    }

    public void SelectSlot(int slot, IPlayer player)
    {
        if (slot < 0)
        {
            slot = _slots.Length - 1;
        }
        if(slot >= _slots.Length)
        {
            slot = 0;
        }

        SelectedSlot = slot;
        _slots[slot].Select(player);
    }


    public void Update(GameTime gameTime, IPlayer player)
    {
        foreach (IHotbarEntry entry in _slots)
        {
            entry.Update(gameTime, player);
        }
    }

    public void Draw(SpriteBatch spriteBatch, IPlayer player)
    {
        foreach (IHotbarEntry entry in _slots)
        {
            entry.Draw(spriteBatch, player);
        }
    }
}
