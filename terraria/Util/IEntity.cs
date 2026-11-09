using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using terraria.Hotbar;
using terraria.Weapons;
using terraria.Collision;
using terraria.Combat;


namespace terraria.Entity;
using static terraria.Constants;



public interface IEntity
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity {get; set;}
    public ICollider Hitbox {get;}
    public bool IsGrounded { get; set; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);

}
