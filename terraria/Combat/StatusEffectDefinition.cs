using System;

namespace terraria.Combat;

public readonly record struct StatusEffectDefinition
{
    public StatusEffectType Type { get; }
    public float Duration { get; }
    public float Strength { get; }

    public StatusEffectDefinition(StatusEffectType type, float duration, float strength)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }
        if (!float.IsFinite(duration) || duration <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        bool invalidStrength = !float.IsFinite(strength) || strength <= 0;
        bool invalidSlow = type == StatusEffectType.Slow && strength >= 1;
        if (invalidStrength || invalidSlow)
        {
            throw new ArgumentOutOfRangeException(nameof(strength));
        }

        Type = type;
        Duration = duration;
        Strength = strength;
    }
}
