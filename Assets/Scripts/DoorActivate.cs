using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorActivate : MonoBehaviour
{
    // Attributes
    [Header("Target Scene parameters")]
    public string targetScene;
    public string targetSpawnPoint;
    [SerializeField]
    public string targetScale = "XS";
    
    public string playerTag = "Player";
 
    private bool activated;
    // Activate 
    void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }
        activated = true;
        // Add data from SceneSwitchData
        SceneSwitchData.SetData(targetScene, targetSpawnPoint, targetScale);
        SceneManager.LoadScene(targetScene);
    }
}
