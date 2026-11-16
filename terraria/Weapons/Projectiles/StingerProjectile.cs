using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;

namespace terraria.Projectiles;

public sealed class StingerProjectile : Projectile
{
    public override CombatTeam Team => CombatTeam.Enemy;

    public StingerProjectile(Texture2D texture, Vector2 position, Vector2 velocity)
        // Classic: one third of stingers carry ten seconds of poison.
        : base(texture, position, velocity,
            Random.Shared.Next(3) == 0 ? TerrariaStats.QueenBeePoisonStinger : TerrariaStats.QueenBeeStinger,
            width: 18f, height: 8f)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // The source sprite points upward; the base class rotates it along flight.
        float pulse = 1f + 0.1f * MathF.Sin(Age * 12f);
        DrawSprite(spriteBatch, pulse);
    }
}
