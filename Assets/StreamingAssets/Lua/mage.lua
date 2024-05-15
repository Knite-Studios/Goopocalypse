-- Unity.OnStart --> Attributes.Set(`from BaseStats`)

BaseStats = {
    name = "Mage",
    health = 70,
    stamina = 100.0,
    speed = 1.6,
    attack_speed = 1.2,
    attack_damage = 13,
    armor = 5,
    aoe = 6.0
}

function OnSpecialAbility(name)
    print(("Player name: ") .. name)
end