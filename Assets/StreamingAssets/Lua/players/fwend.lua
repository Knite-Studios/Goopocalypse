base_stats = {
    name = "Fwend",
    health = 100,
    max_health = 100,
    stamina = 100.0,
    speed = 4.0,
    armor = 8,
    aoe = 3.5,
    camera_distance = 4.0
}

function On_SpecialAbility(context)
    print("Fwend " .. context.name .. " used a special ability.")
end
