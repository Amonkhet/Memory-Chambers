using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Cinemachine;

public class BuildingTimeActivate : MonoBehaviour
{
    // Attribute
    [Header("Activate Range")]
    [SerializeField] float activateRange = 2f;
    [SerializeField] LayerMask buildingTimeMask;
    
    [Header("Animation Timeline")]
    [SerializeField] PlayableDirector playableDirector;
    // playforward if ture, play backward if false
    [SerializeField] bool playForward = true;
    
    [Header("Deactivate Player")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] bool deactivatePlayer = true;
    [SerializeField] MonoBehaviour playerController;
    
    bool isActive = false;
    // Control Camera

    void CheckActivateRange()
    {
        // Check if near activate range
        Transform playerPosition = agent.transform;
        float distance = Vector3.Distance(playerPosition.position, transform.position);
        if (distance > activateRange)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, buildingTimeMask))
        {
            return;
        }
        if (!hit.collider)
        {
            return;
        }

        if (hit.collider.transform != transform && hit.collider.transform.root != transform.root)
        {
            return;
        }
        ActivateBuildingTimeline();
    }

    public void ActivateBuildingTimeline()
    {
        isActive = true;
        // Deactivate player before play animation
    }

}
