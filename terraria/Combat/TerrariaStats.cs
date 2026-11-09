namespace terraria.Combat;

public static class TerrariaStats
{
    public const int PlayerHealth = 100;
    public const int BlueSlimeHealth = 25;
    public const int BlueSlimeContact = 7;
    public const int EyeHealth = 2800;
    public const int EyeContact = 15;
    public const int EyeSecondFormContact = 23;
    public const int QueenBeeHealth = 3400;
    public const int QueenBeeContact = 30;
    public static readonly AttackDefinition WoodenSword = new(7);
    public static readonly AttackDefinition MagicDagger = new(30);
    public static readonly AttackDefinition WoodenBow = new(4 + 5);
    public static readonly AttackDefinition AmethystStaff = new(15);
    public static readonly AttackDefinition WoodenYoyo = new(9);
    public static readonly AttackDefinition LeatherWhip = new(14);
    public static readonly AttackDefinition FireBow = new(4 + 7,
        new StatusEffectDefinition(StatusEffectType.Burning, 3f, 4f));
    public static readonly AttackDefinition FrostStaff = new(15,
        new StatusEffectDefinition(StatusEffectType.Slow, 3f, 0.5f));
}
