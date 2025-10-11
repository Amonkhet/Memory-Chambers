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
        string scale = SceneSwitchData.HasData ? SceneSwitchData.ScaleId    : defaultScale;
        string spawn = SceneSwitchData.HasData ? SceneSwitchData.SpawnPoint : defaultSpawnPoint;
        // Set player to spawn position
        Transform spawnPoint = spawnPointRoot.Find(spawn);
        playerSwitcher.transform.position = spawnPoint.position;
        playerSwitcher.SwitchPlayer(scale);
    }
    
}
