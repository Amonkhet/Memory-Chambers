using UnityEngine;
using UnityEngine.Playables;

public class FinalDoorActivate : MonoBehaviour
{
    private string playerTag = "Player";
    private bool playerInRange = false;
    [SerializeField] private PlayableDirector  timeline; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(1))
        {
            ActivateFinalScene();
        }
    }
    
    private void ActivateFinalScene()
    {
        // Will activate and start final scene timeline
        timeline.Play();
    }
}
