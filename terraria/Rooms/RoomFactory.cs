
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using terraria.Blocks;
using terraria.Bosses;
using static terraria.Constants;


namespace terraria.Rooms;

public class RoomFactory
{

    public List<RoomObjectData> LoadRoom(string fileName)
    {
        List<RoomObjectData> objects = new List<RoomObjectData>();

        string[] lines = File.ReadAllLines(fileName);

        for (int ii = 1; ii < lines.Length; ii++)
        {
            string[] values = lines[ii].Split(',');

            RoomObjectData data = new RoomObjectData();

            data.ObjectType = values[0];
            data.ObjectName = values[1];
            data.Position = values[2];

            objects.Add(data);
        }

        
        //sort list to force heirarchy
        //amke comparator
        objects.Sort((a,b) =>
        {
            Vector2 locA = GetLocation(a.Position);
            Vector2 locB = GetLocation(b.Position);

            int compareX = locA.X.CompareTo(locB.X);

            if (compareX != 0)
            {
                return compareX;
            }//x heirarchy in increasing order first then y

            return locA.Y.CompareTo(locB.Y);
        });

        return objects;
    }
    public object CreateObject(RoomObjectData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Vector2 location = GetLocation(data.Position);
        if (data.ObjectType == "Block")
        {
            return CreateBlock(data.ObjectName, location);
        }

        if (data.ObjectType == "Boss")
        {
            return CreateBoss(data.ObjectName, location);
        }
        throw new Exception("Unknown object type: " + data.ObjectType);
    }

    private IBlock CreateBlock(string objectName, Vector2 location)
    {
        Sprite sprite;

        if (objectName == "Carved-Brick")
        {
            sprite = BlockFactory.Instance.CreateCarvedBrickBlockSprite();
        }
        else if (objectName == "Pearlstone")
        {
            sprite = BlockFactory.Instance.CreatePearlstoneBlockSprite();
        }
        else if (objectName == "Copper-Brick")
        {
            sprite = BlockFactory.Instance.CreateCopperBrickBlockSprite();
        }
        else if (objectName == "Green-Brick")
        {
            sprite = BlockFactory.Instance.CreateGreenBrickBlockSprite();
        }
        else if (objectName == "Red-Brick")
        {
            sprite = BlockFactory.Instance.CreateRedBrickBlockSprite();
        }
        else if (objectName == "Blue-Brick")
        {
            sprite = BlockFactory.Instance.CreateBlueBrickBlockSprite();
        }
        else if (objectName == "Hellstone")
        {
            sprite = BlockFactory.Instance.CreateHellstoneBlockSprite();
            return new SpecialBlock(sprite, location, PlayerMovementEffects.Burning);
        }
        else if (objectName == "Hellstone-Brick")
        {
            sprite = BlockFactory.Instance.CreateHellstoneBrickBlockSprite();
            return new SpecialBlock(sprite, location, PlayerMovementEffects.Burning);
        }
        else if (objectName == "Honey")
        {
            sprite = BlockFactory.Instance.CreateHoneyBlockSprite();
            return new SpecialBlock(sprite, location, PlayerMovementEffects.Slowed);
        }
        else
        {
            throw new Exception("Unknown block: " + objectName);
        }

        return new Block(sprite, location);
    }

     private IBoss CreateBoss(string objectName, Vector2 location)
    {
        IBoss boss;

        if (objectName == "QueenBee")
        {
            boss = BossFactory.Instance.CreateQueenBee();
        }
        else if (objectName == "Cthulhu")
        {
            boss = BossFactory.Instance.CreateCthulhu();
        }
        else if (objectName == "Slime")
        {
            boss = BossFactory.Instance.CreateSlime();
        }
        else
        {
            throw new Exception("Unknown boss: " + objectName);
        }

        boss.Position = location;

        return boss;
    }

    private Vector2 GetLocation(string location)
    {
        string[] values = location.Split(' ');

        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);

        return new Vector2(x, y);
    }

}