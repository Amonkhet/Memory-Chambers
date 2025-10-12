using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Playables;

public class BookActivate : MonoBehaviour
{
    // Attributes
    [Header("Animation and NavMeshLink")]
    public PlayableDirector playableDirector;
    public NavMeshLink navMeshLink;
    [Header("Mechanics key id")]
    [SerializeField] private string mechanicKey = "BookDrop";
    private bool hasTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If this mechanics activated, reset
        if (GameStatus.IsActivated(mechanicKey))
        {
            ResetDoorStatus();
        }
        else
        {
            SetDoorStatus();
        }
    }

    public void ActivateDoor()
    {
        // Play timeline animation
        if (playableDirector)
        {
            playableDirector.time = 0;
            playableDirector.Play();
        }
        // Activate navmeshlink
        if (navMeshLink)
        {
            navMeshLink.enabled = true;
        }
        hasTriggered = true;
        GameStatus.Activated(mechanicKey, true);
    }
    // Make sure when door activated, it will keep the dropped status even when change scene
    private void ResetDoorStatus()
    {
        if (playableDirector)
        {
            playableDirector.time = playableDirector.duration;
            playableDirector.Evaluate();
        }

        if (navMeshLink)
        {
            navMeshLink.enabled = true;
        }
        hasTriggered = true;
    }
    // Make sure if the door is never activated, set it to default
    private void SetDoorStatus()
    {
        if (navMeshLink)
        {
            navMeshLink.enabled = false;
        }
        hasTriggered = false;
    }
}
