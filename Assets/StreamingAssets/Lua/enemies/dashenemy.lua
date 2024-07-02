-- These are loaded when the instance is created.
base_stats = {
    max_health = 20,
    speed = 2,
    armor = 1,
    points = 20,
}

-- Event handler for when a player casts a special ability.
-- context: The 'Hero' object instance which cast the ability.
function On_SpecialAbility(context)
    print("Dash enemy " .. context.Name .. " used a special ability.")
end
