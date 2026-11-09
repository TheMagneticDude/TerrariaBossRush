using System;
using Microsoft.Xna.Framework;

namespace terraria.Combat;

public class HealthComponent
{
    private const float HitFlashDuration = 0.15f;
    private readonly float _invulnerabilityDuration;
    private float _hitFlashTimer;
    private float _invulnerabilityTimer;

    public StatusEffectComponent Effects { get; } = new StatusEffectComponent();
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; }
    public bool IsDead { get; private set; }
    public bool IsInvulnerable { get; set; }
    public bool IsHitFlashing => _hitFlashTimer > 0f;

    public HealthComponent(int maxHealth, float invulnerabilityDuration = 0.75f)
    {
        if (maxHealth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxHealth));
        }

        if (invulnerabilityDuration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(invulnerabilityDuration));
        }

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        _invulnerabilityDuration = invulnerabilityDuration;
    }

    public bool ReceiveHit(AttackDefinition attack)
    {
        ArgumentNullException.ThrowIfNull(attack);
        if (!TakeDamage(attack.Damage))
        {
            return false;
        }

        if (!IsDead)
        {
            foreach (StatusEffectDefinition effect in attack.Effects)
            {
                Effects.Apply(effect);
            }
        }

        return true;
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead || IsInvulnerable)
        {
            return false;
        }

        ApplyDamage(damage);
        _hitFlashTimer = HitFlashDuration;
        _invulnerabilityTimer = _invulnerabilityDuration;
        IsInvulnerable = _invulnerabilityDuration > 0f;
        return true;
    }

    private void ApplyDamage(int damage)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        IsDead = CurrentHealth == 0;
        if (IsDead)
        {
            Effects.Clear();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        int missingHealth = MaxHealth - CurrentHealth;
        CurrentHealth += Math.Min(amount, missingHealth);
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateEffects(seconds);
        UpdateInvulnerability(seconds);
        _hitFlashTimer = Math.Max(0f, _hitFlashTimer - seconds);
    }

    private void UpdateEffects(float seconds)
    {
        if (IsDead)
        {
            return;
        }

        int damage = Effects.Update(seconds);
        ApplyDamage(damage);
    }

    private void UpdateInvulnerability(float seconds)
    {
        if (!IsInvulnerable)
        {
            return;
        }

        _invulnerabilityTimer = Math.Max(0f, _invulnerabilityTimer - seconds);
        IsInvulnerable = _invulnerabilityTimer > 0f;
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
        IsDead = false;
        IsInvulnerable = false;
        _hitFlashTimer = 0f;
        _invulnerabilityTimer = 0f;
        Effects.Clear();
    }
}
