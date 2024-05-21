local rotation_speed = 100.0

function On_MonoUpdate(transform)
    local rotation = CS.UnityEngine.Vector3(0, 0, rotation_speed * CS.UnityEngine.Time.deltaTime)
    transform:Rotate(rotation.x, rotation.y, rotation.z)
end
