base_stats = {
    name = "Fwend",
    health = 100,
    max_health = 100,
    stamina = 100.0,
    speed = 9.5,
    armor = 8,
    aoe = 3.5
}

function On_SpecialAbility(context)
    print("Fwend " .. context.name .. " used a special ability.")
end
