using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    public LayerMask groundMask;
    public float maxRayDistance = 1000f;

    private NavMeshAgent agent;
    private TestinputTemp input;
    private Camera cam;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        input = new TestinputTemp();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (!input.PlayerMove.Move.WasPerformedThisFrame())
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 screenPos = input.PlayerMove.GetPoint.ReadValue<Vector2>();

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                return;
            }
        }

        Ray ray = cam.ScreenPointToRay(screenPos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayDistance, groundMask))
        {
            agent.SetDestination(hit.point);
        }
    }
}