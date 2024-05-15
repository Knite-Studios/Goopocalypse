-- These are loaded when the instance is created.
base_stats = {
    name = "Archer",
    health = 80,
    stamina = 100.0,
    speed = 3.8,
    attack_speed = 1.4,
    attack_damage = 14,
    armor = 6,
    aoe = 0.0
}

-- Event handler for when a player casts a special ability.
-- context: The 'Hero' object instance which cast the ability.
function On_SpecialAbility(context)
    print("Archer " .. context.Name .. " used a special ability.")
end
