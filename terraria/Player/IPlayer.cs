using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using terraria.Hotbar;
using terraria.Weapons;
using terraria.Collision;
using terraria.Combat;
using terraria.Entity;


namespace terraria.Players;
using static terraria.Constants;



public interface IPlayer : IEntity
{
    public ICollider SpecialHitbox {get;set;}
    public PlayerMovementEffects MovementEffects {get;set;}
    bool IsDead { get; }
    void TakeDamage(int damage);
    void Heal(int amount);
    IWeapon EquippedWeapon { get; }
    bool FacingRight { get; }
    int SelectedHotbarSlot { get; }

    PlayerHotbar Hotbar { get; }
    IHotbarEntry SelectedHotbarItem { get; }

    void EquipWeapon(IWeapon weapon);
    void UnequipWeapon();
}
