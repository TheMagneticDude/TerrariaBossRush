using terraria.Weapons;

namespace terraria.Weapons;

public interface IManaWeapon : IWeapon
{
    int ManaCost { get; }
}