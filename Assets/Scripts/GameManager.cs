using UnityEngine;
using UnityEngine.AI;
public class GameManager : MonoBehaviour
{
    // Attributes
    [Header("Player Switcher")]
    public PlayerSwitcher playerSwitcher;
    [Header("Scale and Spawn point")]
    public string defaultScale = "S";
    public Transform spawnPointRoot;
    public string defaultSpawnPoint = "SpawnS";

    void Start()
    {
        // Set player to spawn position
        Transform spawnPoint = spawnPointRoot.Find(defaultSpawnPoint);
        playerSwitcher.transform.position = spawnPoint.position;
        playerSwitcher.SwitchPlayer(defaultScale);
        Vector3 pos = spawnPoint.position;
        NavMeshHit hit;
        float searchRadius = 2f;                   
        int mask = playerSwitcher.agent.areaMask; 
        if (NavMesh.SamplePosition(pos, out hit, searchRadius, mask))
        {
            playerSwitcher.agent.Warp(hit.position);
        }
    }
    
}
