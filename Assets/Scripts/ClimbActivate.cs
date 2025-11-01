using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ClimbActivate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private NavMeshAgent playerAgent;

    [Header("Interaction Settings")]
    [SerializeField] private string playerTag = "Player"; 
    [SerializeField] private KeyCode activateKey = KeyCode.E;
    private bool playerInRange = false;
    private bool climbing = false;

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
        if (playerInRange && !climbing && Input.GetKeyDown(activateKey))
        {
            StartCoroutine(ClimbStair());
        }
    }

    System.Collections.IEnumerator ClimbStair()
    {
        climbing = true;

        playerAgent.isStopped = true;
        playerAgent.updatePosition = false;
        
        Vector3 a = link.transform.TransformPoint(link.startPoint);
        Vector3 b = link.transform.TransformPoint(link.endPoint);
        
        Vector3 playerPos = playerAgent.transform.position;
        bool fromA = (playerPos - a).sqrMagnitude <= (playerPos - b).sqrMagnitude;
        Vector3 startPosition = fromA ? a : b;
        Vector3 endPosition = fromA ? b : a;

        // if use animation, use this
        bool climbUp = endPosition.y > startPosition.y;
        playerAnimator.SetBool("isClimbing", true); // requires a bool parameter in Animator
        playerAnimator.CrossFade(climbUp ? "ClimbUp" : "ClimbDown", 0.1f);

        float duration = 3.0f;
        float t = 0f;
        
        Vector3 dir = (endPosition - startPosition).normalized;
        playerAgent.transform.rotation =
            Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        // Simple move aniamtion
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 pos = Vector3.Lerp(startPosition, endPosition, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 0.3f;    
            playerAgent.transform.position = pos;
            playerAgent.nextPosition = pos;             
            yield return null;
        }

        playerAgent.Warp(endPosition);
        playerAgent.updatePosition = true;
        playerAgent.isStopped = false;
        climbing = false;
    }
}
