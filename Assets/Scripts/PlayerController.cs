using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Nev mesh agent variables
    private NavMeshAgent agent;
    private Animator anim;
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
    [Header("Jump Settings")]
    [SerializeField] float jumpHeightThreshold = 1.0f; // how high must the target be to jump
    [SerializeField] float jumpDuration = 5.9f;        // how long jump animation lasts
    private bool isJumping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isJumping)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitClick))
            {
                EventManager.WhenObjectClicked(hitClick, transform);
            }
        }
        // Check if left mouse clicked every frame
       if (Input.GetMouseButtonDown(0) && !isJumping)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                int mask = agent.areaMask;
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, rangeWalkable, mask))
                {
                    // float heightDiff = navHit.position.y - transform.position.y;
                    // if (heightDiff > jumpHeightThreshold)
                    // {
                    //     StartCoroutine(JumpTo(navHit.position));
                    // }
                    // else
                    // {
                    //     agent.SetDestination(navHit.position);
                    // }
                    agent.SetDestination(navHit.position);
                    if (WhenGroundClicked != null)
                    {
                        WhenGroundClicked.Invoke(navHit.position);
                    }
                }
                else
                {
                    Debug.Log("Clicked point is not a walkable area.");
                }
            }
        }
        float normalizedSpeed = Mathf.InverseLerp(0f, agent.speed, agent.velocity.magnitude);
        anim.SetFloat("Speed", normalizedSpeed);
    }


            
}
