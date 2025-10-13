using System;
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

    [Header("Settings")]
    [SerializeField] private float activateRange = 0.2f;
    [SerializeField] private bool activateOnce = true;
    private bool hasTriggered = false;
    private Transform player;
    // Subscribe to event
    private void OnEnable()
    {
        EventManager.OnObjectClicked += HandleObjectClicked;
    }

    private void OnDisable()
    {
        EventManager.OnObjectClicked -= HandleObjectClicked;
    }

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
    // Handle object clicked
    private void HandleObjectClicked(RaycastHit hit, Transform playerTransform)
    {
        // Check if ray hit this object mechanics
        if (hit.collider.transform.IsChildOf(transform))
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance > activateRange)
            {
                return;
            }
            ActivateDoor();
        }
    }
    public void ActivateDoor()
    {
        if (activateOnce && hasTriggered)
        {
            return;
        }
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
