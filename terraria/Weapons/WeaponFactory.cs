using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using terraria.Combat;
using terraria.Projectiles;

namespace terraria.Weapons;

public class WeaponFactory
{
    private Texture2D _swordTexture;
    private Texture2D _daggerTexture;
    private Texture2D _bowTexture;
    private Texture2D _staffTexture;
    private Texture2D _yoyoTexture;
    private Texture2D _whipTexture;
    private Texture2D _arrowTexture;
    private Texture2D _boltTexture;
    private Texture2D _yoyoHeadTexture;
    private Texture2D _whipSegmentsTexture;
    private Texture2D _stringTexture;

    public static WeaponFactory Instance { get; } = new WeaponFactory();

    private WeaponFactory()
    {
    }

    public void LoadAllTextures(ContentManager content, Texture2D pixelTexture)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(pixelTexture);
        _stringTexture = pixelTexture;
        _swordTexture = content.Load<Texture2D>("Weapons/Sword");
        _daggerTexture = content.Load<Texture2D>("Weapons/Projectiles/Magic_Dagger");
        _bowTexture = content.Load<Texture2D>("Weapons/WoodenBow");
        _staffTexture = content.Load<Texture2D>("Weapons/AmethystStaff");
        _yoyoTexture = content.Load<Texture2D>("Weapons/WoodenYoyo");
        _whipTexture = content.Load<Texture2D>("Weapons/LeatherWhip");
        _arrowTexture = content.Load<Texture2D>("Weapons/Projectiles/WoodenArrow");
        _boltTexture = content.Load<Texture2D>("Weapons/Projectiles/AmethystBolt");
        _yoyoHeadTexture = content.Load<Texture2D>("Weapons/Projectiles/WoodenYoyo");
        _whipSegmentsTexture = content.Load<Texture2D>("Weapons/Projectiles/LeatherWhip");
    }

    public Sword CreateSword()
    {
        return new Sword(_swordTexture);
    }

    public Dagger CreateDagger(ProjectileManager projectiles)
    {
        return new Dagger(_daggerTexture, projectiles);
    }

    public Bow CreateBow(ProjectileManager projectiles)
    {
        return new Bow(_bowTexture, _arrowTexture, projectiles);
    }

    public Staff CreateStaff(ProjectileManager projectiles)
    {
        return new Staff(_staffTexture, _boltTexture, projectiles);
    }

    public Yoyo CreateYoyo()
    {
        return new Yoyo(_yoyoTexture, _yoyoHeadTexture, _stringTexture);
    }

    public Whip CreateWhip()
    {
        return new Whip(_whipTexture, _whipSegmentsTexture);
    }

    public Bow CreateFireBow(ProjectileManager projectiles)
    {
        return new Bow(_bowTexture, _arrowTexture, projectiles, TerrariaStats.FireBow);
    }

    public Staff CreateFrostStaff(ProjectileManager projectiles)
    {
        return new Staff(_staffTexture, _boltTexture, projectiles, TerrariaStats.FrostStaff);
    }
}
