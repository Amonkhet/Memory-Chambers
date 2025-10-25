using UnityEngine;
using UnityEngine.Playables;
public class LeverActivate : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; 

    [Header("Interaction Settings")]
    [SerializeField] private string playerTag = "Player"; 
    [SerializeField] private KeyCode activateKey = KeyCode.Mouse1;
    private bool playerInRange = false;
    private bool activated = false;
    // Trigger when player in range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            Debug.Log("[TimelineDoorActivate] Player entered trigger area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            Debug.Log("[TimelineDoorActivate] Player exited trigger area");
        }
    }

    private void Update()
    {
        if (playerInRange && !activated && Input.GetKeyDown(activateKey))
        {
            activated = true;
            PlayTimeline();
        }
    }
    // Play timeline animation
    private void PlayTimeline()
    {
        if (timeline == null)
        {
            Debug.LogWarning("[TimelineDoorActivate] No timeline assigned!");
            return;
        }
        Debug.Log("[TimelineDoorActivate] Playing timeline...");
        timeline.Play();
    }
}
