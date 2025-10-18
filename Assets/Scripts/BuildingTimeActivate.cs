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
    [SerializeField] MonoBehaviour playerControllerScript;
    
    private bool isActive;
    private bool playerControlEnabled;
    private bool agentEnabled;
    private bool playerPosUpdate;
    private bool playerRotUpdate;
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
        if (playerControllerScript)
        {
            playerControlEnabled = agent.enabled;
            playerControllerScript.enabled = false;
        }
        if (agent)
        {
            if (deactivatePlayer)
            {
                agentEnabled = agent.enabled;
                agent.enabled = false;
            }
            else
            {
                // deactivate agent control
                playerPosUpdate = agent.updatePosition;
                playerRotUpdate = agent.updateRotation;
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.ResetPath();
            }
        }
    }

    void OnBuildingTimelineStopped()
    {
        // Recover player control
        if (playerControllerScript)
            playerControllerScript.enabled = playerControlEnabled;

        // Recover agent control
        if (agent)
        {
            if (deactivatePlayer)
            {
                // Recover agent in new timeline player position
                agent.enabled = agentEnabled;
                agent.Warp(agent.transform.position);
            }
            else
            {
                agent.updatePosition = playerPosUpdate;
                agent.updateRotation = playerRotUpdate;
                agent.isStopped = false;
            }
        }
        isActive = false;
    }
    // Show activate range visually
    void ShowActivateRange()
    {
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawWireSphere(transform.position, activateRange);
    }
}
