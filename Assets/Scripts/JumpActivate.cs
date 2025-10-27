using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Attribute
    [Header("Settings")]
    [SerializeField] private NavMeshAgent playerAgent;     
    [SerializeField] private Animator playerAnimator;
    
    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode activateKey = KeyCode.E;
    
    [Header("Jump Height")]
    [SerializeField] private float jumpHeight;
    
    private bool playerInRange = false;
    private bool jumping = false;
    private bool jumped = false;
    
    
    private NavMeshLink link;
    
    void Awake()
    {
        link = GetComponent<NavMeshLink>();
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
    
    private void Update()
    {
        if (playerInRange && !jumping && Input.GetKeyDown(activateKey))
        {
            StartCoroutine(Jump());
        }
    }
    
    System.Collections.IEnumerator Jump()
    {
        jumping = true;
        if (playerAnimator)
        {
            playerAnimator.Play("HumanoidIdleJumpUp"); 
        }
        
        
        playerAgent.isStopped = true;
        playerAgent.updatePosition = false;
       
        // Setup animation hardcode
        Vector3 startPosition = link.transform.TransformPoint(link.startPoint);
        Vector3 endPosition = link.transform.TransformPoint(link.endPoint);

        float duration = 1.2f;
        float t = 0f;

        Vector3 direction = (endPosition - startPosition).normalized;
        playerAgent.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        float height = jumpHeight + 1f; 

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 position = Vector3.Lerp(startPosition, endPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * height; 
            playerAgent.transform.position = position;
            playerAgent.nextPosition = position;
            yield return null;
        }
        
        playerAgent.Warp(endPosition);
        playerAgent.updatePosition = true;
        playerAgent.isStopped = false;
        jumped = true;
        playerInRange = false;
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        jumping = false;
    }
}
