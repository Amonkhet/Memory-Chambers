using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class ColliderCamera : MonoBehaviour
{
 
    [Header("Settings")]
    [SerializeField] private GameManager gameManager;                
    [SerializeField] private CinemachineVirtualCamera targetCamera;    
    [SerializeField] private int activePriority = 25; 
    
    [Header("Agent type")]
    [SerializeField] private string playerTag = "Player";          
    [SerializeField] private string agentType = "L";         

    private int defualtPriority;  
    private bool isActive = false; 

    private void Reset()
    {
        var collider = GetComponent<Collider>();
        if (collider)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }
        if (CheckAgentType(agentType))
        {
            defualtPriority = targetCamera.Priority;
            targetCamera.Priority = activePriority;
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (!isActive)
        {
            return;
        }
        targetCamera.Priority = defualtPriority;
        isActive = false;
    }

    private void OnDisable()
    {

        if (isActive && targetCamera)
        {
            targetCamera.Priority = defualtPriority;
            isActive = false;
        }
    }

    // Check if the agent is the right type
    private bool CheckAgentType(string expectedType)
    {
        if (!gameManager || gameManager.playerSwitcher == null)
        {
            return false;
        }
        var agent = gameManager.playerSwitcher.agent;
        if (!agent)
        {
            return false;
        }

        string currentType = NavMesh.GetSettingsNameFromID(agent.agentTypeID);
        return currentType == expectedType;
    }
}
