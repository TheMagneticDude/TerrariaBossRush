using System;

namespace terraria.Combat;

internal sealed class ActiveStatusEffect
{
    private double _damageRemainder;

    public double Remaining { get; private set; }
    public float Strength { get; private set; }

    public void Refresh(StatusEffectDefinition effect)
    {
        Remaining = Math.Max(Remaining, effect.Duration);
        Strength = Math.Max(Strength, effect.Strength);
    }

    public int Advance(StatusEffectType type, float seconds)
    {
        double elapsed = Math.Min(seconds, Remaining);
        Remaining -= elapsed;
        return type == StatusEffectType.Slow ? 0 : CalculateDamage(elapsed);
    }

    private int CalculateDamage(double elapsed)
    {
        _damageRemainder += elapsed * Strength;
        int damage = (int)Math.Floor(_damageRemainder);
        _damageRemainder -= damage;
        return damage;
    }
}
