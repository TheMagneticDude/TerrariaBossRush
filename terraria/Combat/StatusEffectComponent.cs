using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace terraria.Combat;

public sealed class StatusEffectComponent
{
    private readonly Dictionary<StatusEffectType, ActiveStatusEffect> _activeEffects =
        new Dictionary<StatusEffectType, ActiveStatusEffect>();

    public float AnimationTimeSeconds { get; private set; }

    public float MovementMultiplier
    {
        get
        {
            if (_activeEffects.TryGetValue(StatusEffectType.Slow, out ActiveStatusEffect slowEffect))
            {
                return 1f - slowEffect.Strength;
            }

            return 1f;
        }
    }

    public bool Has(StatusEffectType type)
    {
        return _activeEffects.ContainsKey(type);
    }

    public Color Tint(Color color)
    {
        if (Has(StatusEffectType.Burning))
        {
            return Color.Lerp(color, Color.OrangeRed, 0.65f);
        }
        if (Has(StatusEffectType.Slow))
        {
            return Color.Lerp(color, Color.LightSkyBlue, 0.65f);
        }
        if (Has(StatusEffectType.Poison))
        {
            return Color.Lerp(color, Color.LimeGreen, 0.65f);
        }
        return color;
    }

    public void Apply(StatusEffectDefinition effect)
    {
        if (effect.Duration <= 0 || effect.Strength <= 0)
        {
            return;
        }

        GetOrCreate(effect.Type).Refresh(effect);
    }

    private ActiveStatusEffect GetOrCreate(StatusEffectType type)
    {
        if (_activeEffects.TryGetValue(type, out var effect))
        {
            return effect;
        }
        effect = new ActiveStatusEffect();
        _activeEffects.Add(type, effect);
        return effect;
    }

    public int Update(float seconds)
    {
        if (!float.IsFinite(seconds) || seconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds));
        }
        AnimationTimeSeconds += seconds;
        int damage = 0;

        foreach (var type in Enum.GetValues<StatusEffectType>())
        {
            if (!_activeEffects.TryGetValue(type, out var effect))
            {
                continue;
            }
            damage += effect.Advance(type, seconds);
            if (effect.Remaining <= 0)
            {
                _activeEffects.Remove(type);
            }
        }
        return damage;
    }

    public void Clear()
    {
        _activeEffects.Clear();
        AnimationTimeSeconds = 0;
    }
}
