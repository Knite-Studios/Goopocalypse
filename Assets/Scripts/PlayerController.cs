using Managers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private PlayerBaseState
    
    
    
    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        var move = InputManager.Movement.ReadValue<Vector2>(); 
        var movement = new Vector3(move.x, move.y, 0);
        transform.position += movement * (Time.deltaTime * 5.0f);
    }
}
