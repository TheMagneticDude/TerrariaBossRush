using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace terraria.Combat;

public sealed class AttackDefinition
{
    public int Damage { get; }
    public IReadOnlyList<StatusEffectDefinition> Effects { get; }
    public Color Tint { get; }

    public AttackDefinition(int damage, params StatusEffectDefinition[] effects)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(damage);
        ArgumentNullException.ThrowIfNull(effects);
        Damage = damage;
        var copiedEffects = (StatusEffectDefinition[])effects.Clone();
        Effects = Array.AsReadOnly(copiedEffects);
        Tint = Color.White;
        foreach (var effect in Effects)
        {
            if (effect.Type == StatusEffectType.Burning)
            {
                Tint = Color.Orange;
            }
            else if (effect.Type == StatusEffectType.Slow)
            {
                Tint = Color.LightSkyBlue;
            }
        }
    }
}
