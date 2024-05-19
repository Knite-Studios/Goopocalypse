base_stats = {
    name = "Buddie",
    health = 100,
    max_health = 100,
    stamina = 100.0,
    speed = 1.0,
    armor = 15,
    aoe = 2.5
}

function On_SpecialAbility(context)
    print("Buddie " .. context.name .. " used a special ability.")
end
