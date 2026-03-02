namespace Systems.Attributes
{
    /// <summary>
    /// Attribute types.
    /// Shared stats sit at the top; player-only and enemy-only are grouped.
    /// New attributes can be added freely -- the modifier system picks them up automatically.
    /// </summary>
    public enum Attribute
    {
        // Shared
        MaxHealth,
        Speed,
        Armor,

        // Player-only
        Stamina,
        AreaOfEffect,
        CameraDistance,
        HealthRegen,
        DamageMultiplier,
        CooldownReduction,

        // Enemy-only
        Points,
    }
}
