using System;

namespace terraria.Combat;

public class ManaComponent
{
    public int CurrentMana { get; private set; }
    public int MaxMana { get; }

    public ManaComponent(int maxMana)
    {
        if (maxMana <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxMana));
        }

        MaxMana = maxMana;
        CurrentMana = maxMana;
    }

    public bool UseMana(int amount)
    {
        if (amount <= 0 || CurrentMana < amount)
        {
            return false;
        }

        CurrentMana -= amount;
        return true;
    }

    public void RestoreMana(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        CurrentMana = Math.Min(MaxMana, CurrentMana + amount);
    }

    public void Reset()
    {
        CurrentMana = MaxMana;
    }

}