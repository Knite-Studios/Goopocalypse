-- These are loaded when the instance is created.
base_stats = {
    max_health = 20,
    speed = 2,
    armor = 1,
    points = 60,
}

-- Event handler for when a player casts a special ability.
-- context: The 'Hero' object instance which cast the ability.
function On_SpecialAbility(context)
    print("Ranged enemy " .. context.Name .. " used a special ability.")
end
