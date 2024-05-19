-- These are loaded when the instance is created.
base_stats = {
    health = 20,
    speed = 3.8,
    attack_damage = 10,
    armor = 10
}

-- Event handler for when a player casts a special ability.
-- context: The 'Hero' object instance which cast the ability.
function On_SpecialAbility(context)
    print("Archer " .. context.Name .. " used a special ability.")
end
