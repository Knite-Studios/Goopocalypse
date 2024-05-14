using Managers;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // private PlayerBaseState
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpawnDistance = 1.0f;
    
    private void Update()
    {
        if (!isLocalPlayer) return;
        
        HandleMovement();
        HandleAttack();
    }
    
    private void HandleMovement()
    {
        var move = InputManager.Movement.ReadValue<Vector2>(); 
        var movement = new Vector3(move.x, move.y, 0);
        transform.position += movement * (Time.deltaTime * 5.0f);
    }
    
    private void HandleAttack()
    {
        if (InputManager.Attack.WasReleasedThisFrame())
        {
            var mousePos = Camera.main!.ScreenToWorldPoint(InputManager.Mouse.ReadValue<Vector2>());
            mousePos.z = 0;
            
            var direction = (mousePos - transform.position).normalized;
            var spawnPosition = transform.position + (direction * projectileSpawnDistance);
            
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(0, 0, angle);
            
            CmdSpawnProjectile(spawnPosition, rotation);
        }
    }
    
    [Command]
    private void CmdSpawnProjectile(Vector3 position, Quaternion rotation)
    {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        NetworkServer.Spawn(projectile);
    }
}
