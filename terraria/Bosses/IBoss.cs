using terraria.Collision;
using terraria.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Entity;

namespace terraria.Bosses;

public interface IBoss : IEntity
{
    HealthComponent Health { get; }
    int ContactDamage { get; } 
}
