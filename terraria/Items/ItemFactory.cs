using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;


namespace terraria.Items;

public class ItemFactory
{
    private TextureAtlas _healthPotAtlas;
    private TextureAtlas _recallPotAtlas;
    

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
        _healthPotAtlas = TextureAtlas.FromFile(content, "Items/healthPotion.xml");
        _recallPotAtlas = TextureAtlas.FromFile(content, "Items/recallPotion.xml");

    }

    public HealthPotion CreateHealthPot()
    {
        return new HealthPotion(_healthPotAtlas);
    }
    public RecallPotion CreateRecallPot()
    {
        return new RecallPotion(_recallPotAtlas);
    }
}
