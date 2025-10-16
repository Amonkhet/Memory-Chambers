using System;
using UnityEngine;
using UnityEngine.AI;
public class BuildingMove : MonoBehaviour
{
    // Attribute
    [Header("Building pushed distance")]
    [SerializeField] float pushDistance;
    [Header("Pushable activate range")]
    [SerializeField] float activateRange;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                PushBuilding();
            }
        }
    }
    // Push the building
    void PushBuilding()
    {
        // Get current agent scale from player switcher
        var agent = PlayerSwitcher.CurrentAgent;
        var player = PlayerSwitcher.CurrentPlayerRoot;
        // Player distance from building
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > activateRange)
        {
            return;
        }
        // Player direction
        Vector3 direction = player.forward;
        direction.Normalize();
        // Building location
        Vector3 targetBuilding = transform.position + direction * pushDistance;
        // Move building draft
        transform.position = targetBuilding;
        // Update obstcel for navmesh surfaces
        var obstcel = GetComponent<NavMeshObstacle>();
        if (obstcel)
        {
            obstcel.carving = true;
        }
        // Check if the front has collider, if yes, avoid collide
        if (Physics.CheckBox(targetBuilding, Vector3.one * 0.9f))
        {
            return;
        }
    }
}
