
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using static terraria.Constants;
using terraria.Collision;

using System;


namespace terraria.Blocks;

public class SpecialBlock: IBlock
{
    public Vector2 Position { get; set; }

    public Sprite TheSprite { get; set; }

    public RectangularCollider Hitbox {get;set;}
    public PlayerMovementEffects MovementEffect {get;set;}

    public void Update(GameTime gameTime)
    {
        Hitbox.Update(Position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        TheSprite.Draw(spriteBatch, Position);
        Hitbox.DrawDebug(spriteBatch);
    }

    public SpecialBlock(Sprite sprite, Vector2 location, PlayerMovementEffects effectType)
    {
        ArgumentNullException.ThrowIfNull(sprite);

        this.TheSprite = sprite;
        this.Position = location;
        MovementEffect = effectType;

        Hitbox = new RectangularCollider(
        Position,
        sprite.Width,
        sprite.Height
        );
    }

}