using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoorActivate : MonoBehaviour
{
    // Attributes
    [Header("Target Scene parameters")]
    public string targetScene;
    public string targetSpawnPoint;
    [SerializeField]
    public string targetScale = "XS";
    public string playerTag = "Player";
    private bool activated;
    [Header("Key binds")]
    [SerializeField] KeyCode activateKey = KeyCode.Mouse1;
    bool playerInRange = false;
    Collider playerCollider;
    
    // Activate 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerCollider = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerCollider = null;
        }
    }

    void Update()
    {
        if (playerInRange && !activated && Input.GetKeyDown(activateKey))
        {
            activated = true;
            // Trigger event
            EventManager.WhenEnterDoor(new EventManager.DoorToScene(targetScene, targetSpawnPoint, targetScale));
        }
    }
}
