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
    
    private bool playerInRange = false;
    private bool jumping = false;
    
    
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

        playerAgent.isStopped = true;
        playerAgent.updatePosition = false;
        // Set start and end point using mesh link
        Vector3 a = link.transform.TransformPoint(link.startPoint);
        Vector3 b = link.transform.TransformPoint(link.endPoint);
       
        // Setup animation hardcode
        Vector3 playerPosition = playerAgent.transform.position;
        bool fromA = (playerPosition - a).sqrMagnitude <= (playerPosition - b).sqrMagnitude;
        Vector3 startPosition = fromA ? a : b;
        Vector3 endPosition = fromA ? b : a;

        float duration = 1.2f;
        float t = 0f;

        Vector3 direction = (endPosition - startPosition).normalized;
        playerAgent.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        float height = Mathf.Abs(endPosition.y - startPosition.y) + 1.0f; 

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
        jumping = false;
    }
}
