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

        Vector3 startPosition = link.transform.TransformPoint(link.startPoint);
        Vector3 endPosition   = link.transform.TransformPoint(link.endPoint);

        float duration = 1.0f; 
        float t = 0f;
        Vector3 pos;
        
        Vector3 dir = (endPosition - startPosition).normalized;
        playerAgent.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 0.3f;
            playerAgent.transform.position = pos;
            yield return null;
        }
        
        playerAgent.Warp(endPosition);

        playerAgent.updatePosition = true;
        playerAgent.isStopped = false;
        climbing = false;
    }
}
