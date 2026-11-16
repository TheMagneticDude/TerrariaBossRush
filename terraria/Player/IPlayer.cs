using terraria.Hotbar;
using terraria.Weapons;
using terraria.Collision;
using terraria.Entity;
using static terraria.Constants;

namespace terraria.Players;

public interface IPlayer : IEntity
{
    ICollider SpecialHitbox { get; }
    void SetBlockContactEffect(PlayerMovementEffects effect);
    bool IsDead { get; }
    void TakeDamage(int damage);
    void Heal(int amount);
    IWeapon EquippedWeapon { get; }
    bool FacingRight { get; }

    PlayerHotbar Hotbar { get; }

    void EquipWeapon(IWeapon weapon);
    void UnequipWeapon();

    int CurrentHealth { get; }
    int MaxHealth { get; }
}
