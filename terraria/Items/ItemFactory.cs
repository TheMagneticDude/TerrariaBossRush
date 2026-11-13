using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;


namespace terraria.Items;

public class ItemFactory
{
    private Texture2D _healthPotTexture;
    private Texture2D _recallPotTexture;
    private Texture2D _manaPotTexture;
    private Texture2D _swiftPotTexture;
    
    private static ItemFactory instance = new ItemFactory();

    public static ItemFactory Instance
    {
        get
        {
            return instance;
        }
    }

    private ItemFactory()
    {
        
    }
    
    public void LoadAllTextures(ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);
        _healthPotTexture = content.Load<Texture2D>("Items/healthPotion");
        _recallPotTexture = content.Load<Texture2D>("Items/recallPotion");
        _manaPotTexture = content.Load<Texture2D>("Items/manaPotion");
        _swiftPotTexture = content.Load<Texture2D>("Items/swiftnessPotion");

    }

    public HealthPotion CreateHealthPot()
    {
        return new HealthPotion(_healthPotTexture);
    }
    public RecallPotion CreateRecallPot()
    {
        return new RecallPotion(_recallPotTexture);
    }
    public ManaPotion CreateManaPot()
    {
        return new ManaPotion(_manaPotTexture);
    }
     public SwiftnessPotion CreateSwiftPot()
    {
        return new SwiftnessPotion(_swiftPotTexture);
    }
}
