using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using terraria.Hotbar;
using terraria.Weapons;
using terraria.Collision;
using terraria.Combat;


namespace terraria.Players;
using static terraria.Constants;



public interface IPlayer
{
    public Vector2 Location { get; set; }
    public Vector2 Velocity {get; set;}
    public CapsuleCollider Hitbox {get; set;}    
    public ICollider SpecialHitbox {get;set;}
    public bool IsGrounded { get; set; }
    public PlayerMovementEffects MovementEffects {get;set;}
    bool IsDead { get; }
    void TakeDamage(int damage);
    void Heal(int amount);
    IWeapon EquippedWeapon { get; }
    bool FacingRight { get; }
    int SelectedHotbarSlot { get; }

    PlayerHotbar Hotbar { get; }
    IHotbarEntry SelectedHotbarItem { get; }
    
    void Update(GameTime gameTime, PlayerInput input);
    void Draw(SpriteBatch spriteBatch);

    void EquipWeapon(IWeapon weapon);
    void UnequipWeapon();

    int CurrentHealth { get; }
    int MaxHealth { get; }
}
