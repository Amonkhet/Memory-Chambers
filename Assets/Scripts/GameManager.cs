using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Attributes
    [Header("Player Switcher")]
    public PlayerSwitcher playerSwitcher;
    [Header("Scale and Spawn point")]
    public string defaultScale = "S";
    public Transform spawnPointRoot;
    public string defaultSpawnPoint = "SpawnS";
    // Subscribe to event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Find what scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene == gameObject.scene)
        {
            StartCoroutine(LocateToSpawn());
        }
    }
    void Start()
    {
        // Make sure the script is activated in this loaded scene
        if (gameObject.scene == SceneManager.GetActiveScene())
        {
            StartCoroutine((LocateToSpawn()));
        }
    }
    // Wait a few frame and locate player model to corresponding spawn point
    IEnumerator LocateToSpawn()
    {
        NavMeshAgent agent;
        if (playerSwitcher != null)
        {
            agent = playerSwitcher.agent;
        }
        else
        {
            agent = null;
        }
        if (agent != null) { agent.isStopped = true; agent.ResetPath(); }
        // When trun to webgl build, wait three frame to allow webgl load completely
#if UNITY_WEBGL
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }  
#else
        yield return null;                              
#endif
        string scale = SceneSwitchData.HasData ? SceneSwitchData.ScaleId    : defaultScale;
        string spawn = SceneSwitchData.HasData ? SceneSwitchData.SpawnPoint : defaultSpawnPoint;
        // Find spawn point
        Transform spawnPoint = null;
        if (spawnPointRoot)
        {
            if (!string.IsNullOrEmpty(spawn))
            {
                spawnPoint = spawnPointRoot.Find(spawn);
            }
            if (spawnPoint == null && !string.IsNullOrEmpty(defaultSpawnPoint))
            {
                spawnPoint = spawnPointRoot.Find(defaultSpawnPoint);
            }
        }
        // Disable nav mesh agent before transform and change spawn point
        bool agentEnabled = agent != null && agent.enabled;
        if (agentEnabled) agent.enabled = false;

        if (spawnPoint)
        {
            playerSwitcher.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
        playerSwitcher.SwitchPlayer(scale);
        // Add one frame load time for scene to switch player and attributes
#if UNITY_WEBGL
        yield return null; 
#endif

        if (agentEnabled) agent.enabled = true;

        if (agent != null)
        {
            agent.Warp(playerSwitcher.transform.position);
            agent.ResetPath();
            agent.isStopped = false;
        }

        if (SceneSwitchData.HasData) SceneSwitchData.ClearData();
    }
}
