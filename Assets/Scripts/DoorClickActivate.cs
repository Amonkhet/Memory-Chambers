using UnityEngine;
using UnityEngine.AI;
public class DoorClickActivate : MonoBehaviour
{
    [Header("Door ID")]
    public string doorID;
    
    [Header("Door Switcher manager")]
    public BuildingDoorSwitcher doorSwitcher;
    
    [Header("Activate range settings")]
    [SerializeField] float activateRange = 2f;     
    [SerializeField] private LayerMask doorLayerMask;
    public NavMeshAgent agent;              
    public Collider doorCollider; 
    
    [Header("Timeline manager")] 
    public WorkTimelineManager manager;

    [Header("TimelineRise or Fall")] 
    public bool playRiseTimeline = true;
    
    [Header("Play direction")]
    public bool playBackward = false;
    
    [Header("Activate key")]
    public KeyCode activateKey = KeyCode.Mouse1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    // Get player position
    void Awake()
    {
        if (!agent && PlayerSwitcher.CurrentAgent)
        {
            agent = PlayerSwitcher.CurrentAgent;
        }

        if (!doorCollider)
        {
            doorCollider = GetComponent<Collider>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        // right click to activate
        if (Input.GetKeyDown(activateKey))
        {
            Vector3 playerPosition = agent.transform.position;
            Vector3 near = doorCollider.ClosestPoint(playerPosition);
            float distanceXZ = Vector2.Distance(new Vector2(playerPosition.x, playerPosition.z),
                new Vector2(near.x, near.z));
            if (distanceXZ > activateRange)
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                return;
            }
            if (!(hit.collider && (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))))
            {
                return;
            }
            if (doorSwitcher)
            {
                doorSwitcher.SwitchDoor(doorID);
            }

            if (playRiseTimeline)
            {
                manager.PlayTimelineRise(playBackward);
            }
            else
            {
                manager.PlayTimelineFall(playBackward);
            }
        }
    }
    // Door disable/enable door collider interact
    public void DisableDoorInteract()
    {
        if (doorCollider)
        {
            doorCollider.enabled = false;
        }
    }

    public void EnableDoorInteract()
    {
        if (doorCollider)
        {
            doorCollider.enabled = true;
        }
    }
    void OnDrawGizmosSelected()
    {
        if (!doorCollider) return;
        Gizmos.color = new Color(1, 0, 0, 0.7f);
        Vector3 center = doorCollider.bounds.center;
        Gizmos.DrawWireSphere(new Vector3(center.x, transform.position.y, center.z), activateRange);
    }
}
