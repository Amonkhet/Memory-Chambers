using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Nev mesh agent variables
    private NavMeshAgent agent;
    // Range to find walkable point
    [Header("Control Settings")]
    [SerializeField] float rangeWalkable = 0.5f;
    // Select the Ground layer for ray hit
    [SerializeField] LayerMask groundLayer;
    // Move speed of the player
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 10f;
    // Event listener to get click position and for adding future animation/effects
    public static event System.Action<Vector3> WhenGroundClicked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if left mouse clicked every frame
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // If the ray cast by left click is on baked nev mesh
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, rangeWalkable, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                    // Interface for animation and click effects
                    if (WhenGroundClicked != null)
                    {
                        WhenGroundClicked.Invoke(navHit.position);
                    }
                }
            }
        }
    }
}
