using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        
        var movement = new Vector3(horizontal, vertical, 0);
        transform.position += movement * Time.deltaTime * 5.0f;
    }
}
