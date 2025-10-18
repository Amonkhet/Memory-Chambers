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
    [SerializeField] Collider doorCollider;
    
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

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CheckActivateRange();
        }
    }
    // Calculate player distance range to door
    float CalculateActivateRange()
    {
        if (doorCollider)
        {
            Vector3 player = agent.transform.position;
            Vector3 closetPoint = doorCollider.ClosestPoint(player);
            return Vector2.Distance(new Vector2(player.x, player.z), new Vector2(closetPoint.x, closetPoint.z));
        }

        return 0;
    }
    void CheckActivateRange()
    {
        // Check if near activate range
        // Transform playerPosition = agent.transform;
        float distance = CalculateActivateRange();
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
        if (playForward)
        {
            playableDirector.time = 0;
        }
        else
        {
            playableDirector.time = playableDirector.duration;

        }
        playableDirector.Evaluate();
        playableDirector.Play();
        // Set play forward or backward
        var graph = playableDirector.playableGraph;
        if (graph.IsValid())
        {
            if (playForward)
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
            else
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(-1);
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

}
