using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
public class LeverActivate : MonoBehaviour
{
    [Header("Agent Type")]
    [SerializeField] private string targetAgent = "S"; 
    [SerializeField] private GameObject hintDenied;   
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
        }
        var switcher = other.GetComponentInParent<PlayerSwitcher>();
        if (switcher != null && switcher.currentId != targetAgent)
        {
            playerInRange = false;
            if (hintDenied) hintDenied.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
        // Deactivate denied hint
        if (hintDenied)
        {
            hintDenied.SetActive(false);
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
            return;
        }
        timeline.Play();
    }
}
