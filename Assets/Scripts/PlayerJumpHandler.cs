using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJumpHandler : MonoBehaviour
{
    public Animator animator;
    public float jumpForce = 5f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Jump()
    {
        Debug.Log("Jump triggered!");

        // Play jump animation
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        // Add upward force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}

