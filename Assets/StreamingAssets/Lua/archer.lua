-- Unity.OnStart --> Attributes.Set(`from BaseStats`)

BaseStats = {
    name = "Archer",
    health = 80,
    stamina = 100.0,
    speed = 3.8,
    attack_speed = 1.4,
    attack_damage = 14,
    armor = 6,
    aoe = 0.0
}

function OnSpecialAbility(name)
    print(("Player name: ") .. name)
end