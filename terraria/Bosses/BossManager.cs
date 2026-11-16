using System.Collections.Generic;
using terraria.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace terraria.Bosses;

public class BossManager
{
    public List<IBoss> Bosses { get; private set; }

    public BossManager()
    {
        Bosses = new List<IBoss>();
    }

    public void Add(IBoss Boss)
    {
        Bosses.Add(Boss);
    }

    public void UpdateAll(GameTime gameTime, Vector2 playerPos)
    {
        foreach (IBoss Boss in Bosses)
        {
            Boss.Update(gameTime);
        }
    }

    public void DrawAll(SpriteBatch spriteBatch, StatusEffectVisuals effectVisuals)
    {
        System.ArgumentNullException.ThrowIfNull(effectVisuals);
        foreach (IBoss Boss in Bosses)
        {
            Boss.Draw(spriteBatch);
            if (!Boss.Health.IsDead)
            {
                effectVisuals.Draw(spriteBatch, Boss.Hitbox.BoundingBox, Boss.Health.Effects);
            }
        }
    }

    public void Clear()
    {
        Bosses.Clear();
    }

    public int CurrentHealth
    {
        get
        {
            int total = 0;
            foreach (IBoss boss in Bosses)
            {
                if (!boss.Health.IsDead)
                {
                    total += boss.Health.CurrentHealth;
                }
            }

            return total;
        }
    }

    public int MaxHealth
    {
        get
        {
            int total = 0;

            foreach (IBoss boss in Bosses)
            {
                total += boss.Health.MaxHealth;
            }

            return total;
        }
    }

}
