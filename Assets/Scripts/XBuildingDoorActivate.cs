using UnityEngine;
using UnityEngine.AI;

public class XBuildingDoorActivate : MonoBehaviour
{
    public DoorTiggerID doorID = DoorTiggerID.A;          
    public XBuildingTimelineManager manager;           
    public NavMeshAgent agent;              
    public Collider doorCollider;          

    [Header("Activate range settings")]
    [SerializeField] float activateRange = 2f;     
    [SerializeField] private LayerMask doorLayerMask;

    void Awake()
    {
        if (!agent && PlayerSwitcher.CurrentAgent) agent = PlayerSwitcher.CurrentAgent;
        if (!doorCollider) doorCollider = GetComponent<Collider>();
        if (!manager) manager = FindAnyObjectByType<BuildingTimelineManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ActivateDoor();
        }
    }

    void ActivateDoor()
    {
        if (!manager || !agent || !doorCollider)
        {
            return;
        }
        
        Vector3 playerPosition = agent.transform.position;
        Vector3 near = doorCollider.ClosestPoint(playerPosition);
        float distanceXZ = Vector2.Distance(new Vector2(playerPosition.x, playerPosition.z),
                                        new Vector2(near.x, near.z));
        if (distanceXZ > activateRange)
        {
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, doorLayerMask)) return;
        if (!hit.collider) return;
        if (hit.collider.transform != transform && hit.collider.transform.root != transform.root) return;
        
        manager.ActivateDoor(doorID);
    }

    // Show activate range
    void OnDrawGizmosSelected()
    {
        if (!doorCollider) return;
        Gizmos.color = new Color(1, 0, 0, 0.7f);
        Vector3 center = doorCollider.bounds.center;
        Gizmos.DrawWireSphere(new Vector3(center.x, transform.position.y, center.z), activateRange);
    }
}