using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using terraria.Entity;
using System.Collections.Generic;

namespace terraria.Bosses;

public class BossFactory
{
    private TextureAtlas _queenBeeAtlas;

    private TextureAtlas _cthulhuAtlas;

    private TextureAtlas _slimeAtlas;

    public IEntity TargetPlayer { get; set; }

    private static BossFactory instance = new BossFactory();
    
    public List<IBoss> ActiveBosses { get; private set; }

    public static BossFactory Instance
    {
        get
        {
            return instance;
        }
    }

    private BossFactory()
    {
        
    }

    public void LoadAllTextures(ContentManager content)
    {
        _queenBeeAtlas = TextureAtlas.FromFile(content,"Bosses/QueenBee.xml");
        _cthulhuAtlas = TextureAtlas.FromFile(content, "Bosses/Cthulhu.xml");
        _slimeAtlas = TextureAtlas.FromFile(content, "Bosses/Slime.xml");
    }

    public QueenBee CreateQueenBee()
    {
            AnimatedSprite sprite = _queenBeeAtlas.CreateAnimatedSprite("queen-bee-idle");

            sprite.CenterOrigin();

            return new QueenBee(sprite, _queenBeeAtlas, TargetPlayer);
    }

    public EyeOfCthulhu CreateCthulhu()
    {
        AnimatedSprite sprite = _cthulhuAtlas.CreateAnimatedSprite("cthulhu-phase-1");

        sprite.CenterOrigin();

        return new EyeOfCthulhu(sprite, _cthulhuAtlas, TargetPlayer);
    }

    public Slime CreateSlime()
    {
            AnimatedSprite sprite = _slimeAtlas.CreateAnimatedSprite("slime");

            sprite.CenterOrigin();

            return new Slime(sprite, _slimeAtlas, TargetPlayer);
    }

    public Slime CreateColoredSlime(Color c)
    {
            AnimatedSprite sprite = _slimeAtlas.CreateAnimatedSprite("slime");

            sprite.CenterOrigin();

            return new Slime(sprite, _slimeAtlas, c, TargetPlayer);
    }

}