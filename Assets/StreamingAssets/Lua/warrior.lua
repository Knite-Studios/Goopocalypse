-- Unity.OnStart --> Attributes.Set(`from BaseStats`)

BaseStats = {
    name = "Warrior",
    health = 100,
    stamina = 100.0,
    speed = 1.1,
    attack_speed = 1.0,
    attack_damage = 10,
    armor = 10,
    aoe = 4.0
}

function OnSpecialAbility(name)
    print(("Player name: ") .. name)
end