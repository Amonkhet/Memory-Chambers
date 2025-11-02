using UnityEngine;

public class JumpZone : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Find the player in the scene
        PlayerJumpHandler player = FindObjectOfType<PlayerJumpHandler>();
        if (player != null)
        {
            player.Jump();
        }
    }
}
