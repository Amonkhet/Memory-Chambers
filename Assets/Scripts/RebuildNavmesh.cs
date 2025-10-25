using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class RebuildNavmesh : MonoBehaviour
{
    // Rebuild nav mesh when room moved
    [Header("Navmesh settings")]
    [SerializeField] PlayableDirector director;
    [SerializeField] NavMeshSurface meshXS;
    [SerializeField] private GameManager gameManager;         
    [SerializeField] private string agentXS = "XS";  
    [SerializeField] private LayerMask layerMask;


    private void Awake()
    {
        if (director)
        {
            director.stopped += OnTimelineStopped;
        }
    }

    private void OnDestroy()
    {
        if (director)
        {
            director.stopped -= OnTimelineStopped;
        }
    }

    private void OnTimelineStopped(PlayableDirector d)
    {
        var agent = gameManager.playerSwitcher.agent;
        string currentAgentTypeName = NavMesh.GetSettingsNameFromID(agent.agentTypeID);

        // Rebuild when agent is XS
        if (currentAgentTypeName == agentXS)
        {
            meshXS.layerMask = layerMask;
            meshXS.BuildNavMesh();
        }
    }
}
