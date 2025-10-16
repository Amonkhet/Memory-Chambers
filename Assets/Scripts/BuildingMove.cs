using System;
using UnityEngine;
using UnityEngine.AI;
public class BuildingMove : MonoBehaviour
{
    // Attribute
    [Header("Building pushed distance")]
    [SerializeField] float pushDistance = 2.5f;
    [Header("Pushable activate range")]
    [SerializeField] float activateRange = 2.8f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        // Player position related to building
        Vector3 playerBuildingPos = player.position - transform.position;
        playerBuildingPos.y = 0;
        Vector3 direction;
        // Restrict building to only move follow x or z axis in stright line
        if (Mathf.Abs(playerBuildingPos.x) >= Mathf.Abs(playerBuildingPos.z))
        {
            // Move to the opposite side of the relative position of player to builiding
            direction = new Vector3(-Mathf.Sign(playerBuildingPos.x), 0f, 0f);
        }
        else
        {
            direction = new Vector3(0f, 0f, -Mathf.Sign(playerBuildingPos.z));
        }
        // Building location (with height fixed)
        Vector3 targetBuilding = transform.position + direction * pushDistance;
        targetBuilding.y = transform.position.y;
        // Check if the front block has collider
        Vector3 checkBox = new Vector3(1.25f, 2f, 1.25f) * 0.9f;
        if (Physics.CheckBox(targetBuilding, checkBox, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore))
        {
            return;
        }
        // Update obstcel for navmesh surfaces
        var obstcel = GetComponent<NavMeshObstacle>();
        if (obstcel)
        {
            obstcel.carving = false;
        }
        // Move building draft
        transform.position = targetBuilding;
        if (obstcel)
        {
            obstcel.carving = true;
        }
    }
}
