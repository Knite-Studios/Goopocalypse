base_stats = {
    name = "Fwend",
    health = 100,
    stamina = 100.0,
    speed = 1.5,
    attack_speed = 1.2,
    attack_damage = 12,
    armor = 8,
    aoe = 3.5
}

function On_SpecialAbility(context)
    print("Fwend " .. context.name .. " used a special ability.")
end
