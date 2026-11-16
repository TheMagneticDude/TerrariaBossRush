using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using terraria.Weapons;

namespace terraria.Projectiles;

public class ProjectileFactory
{
    private Texture2D _daggerTexture;
    private Texture2D _stingerTexture;

    private static readonly ProjectileFactory instance = new ProjectileFactory();

    public static ProjectileFactory Instance
    {
        get
        {
            return instance;
        }
    }

    private ProjectileFactory()
    {
        
    }
    
    public void LoadAllTextures(ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);
        _daggerTexture = content.Load<Texture2D>("Weapons/Projectiles/Magic_Dagger");
        _stingerTexture = content.Load<Texture2D>("Weapons/Projectiles/QueenBeeStinger");
    }

    public StingerProjectile CreateStinger(Vector2 position, Vector2 velocity)
    {
        return new StingerProjectile(_stingerTexture, position, velocity);
    }

    public DaggerProjectile CreateDagger(Vector2 position, Vector2 velocity)
    {
        return new DaggerProjectile(_daggerTexture, position, velocity);
    }
}