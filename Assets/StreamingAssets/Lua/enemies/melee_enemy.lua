-- These are loaded when the instance is created.
base_stats = {
    health = 20,
    max_health = 20,
    speed = 9.8,
    armor = 1,
}

-- Event handler for when a player casts a special ability.
-- context: The 'Hero' object instance which cast the ability.
function On_SpecialAbility(context)
    print("Archer " .. context.Name .. " used a special ability.")
end
