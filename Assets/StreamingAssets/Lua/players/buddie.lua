base_stats = {
    name = "Buddie",
    health = 100,
    max_health = 100,
    stamina = 100.0,
    speed = 3.5,
    armor = 15,
    aoe = 2.5,
    camera_distance = 5.5
}

function On_SpecialAbility(context)
    print("Buddie " .. context.name .. " used a special ability.")
end
