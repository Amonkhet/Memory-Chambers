using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Playables;

public class BookDropActivate : MonoBehaviour
{
    // Attributes
    [Header("Animation and NavMeshLink")]
    public PlayableDirector playableDirector;
    public NavMeshLink navMeshLink;
    [Header("Mechanics key id")]
    [SerializeField] private string mechanicKey = "BookDrop";
    [Header("Animated model")]
    public Transform transformModel;
    public Transform rotateModel;
    [Header("Activate range")]
    [SerializeField] private float activateRange = 0.2f;
    [SerializeField] private bool activateOnce = true;
    private bool hasTriggered = false;
    private Transform player;
    public static bool Dropped = false;
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
            ResetBookStatus();
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
    // Save the dropped book status once mechanics activated
    private void BookActivated(PlayableDirector playableDirector)
    {
        if (transformModel)
        {
            // Add a "_root" to differentiate key place, transformation is on the root
            GameStatus.SaveBookDroppedStatus(mechanicKey + "_root", transformModel);
        }

        if (rotateModel)
        {
            GameStatus.SaveBookDroppedStatus(mechanicKey + "_child", rotateModel);
        }
        // Do not activated once animation stopped
        playableDirector.stopped -= BookActivated;
    }
    // Activate door mechanics
    public void ActivateDoor()
    {
        if (activateOnce && hasTriggered)
        {
            return;
        }
        // Play timeline animation
        if (playableDirector)
        {
            playableDirector.stopped -= BookActivated;
            playableDirector.stopped += BookActivated;
            
            playableDirector.playOnAwake = false;
            playableDirector.time = 0;
            playableDirector.Play();
        }
        // Activate navmeshlink
        if (navMeshLink)
        {
            navMeshLink.enabled = true;
        }
        hasTriggered = true;
        Dropped = true;
        GameStatus.Activated(mechanicKey, true);
    }
    // Make sure when door activated, it will keep the dropped status even when change scene
    private void ResetBookStatus()
    {
        bool reseted = false;
        if (navMeshLink)
        {
            navMeshLink.enabled = true;
        }
        // Reset book transformation status
        if (transformModel && GameStatus.GetBookDroppedStatus(mechanicKey + "_root", out var transform))
        {
            transformModel.localPosition = transform.droppedPosition;
            transformModel.localEulerAngles = transform.droppedRotation;
            transformModel.localScale = transform.droppedScale;
            reseted = true;
        }
        // Reset book rotation status
        if (rotateModel && GameStatus.GetBookDroppedStatus(mechanicKey + "_child", out var rotate))
        {
            rotateModel.localPosition = rotate.droppedPosition;
            rotateModel.localEulerAngles = rotate.droppedRotation;
            rotateModel.localScale = rotate.droppedScale;
            reseted = true;
        }
        // Check if reseted
        if (reseted)
        {
            hasTriggered = true;
        }

        if (transformModel)
        {
            GameStatus.SaveBookDroppedStatus(mechanicKey + "_root", transformModel);
        }

        if (rotateModel)
        {
            GameStatus.SaveBookDroppedStatus(mechanicKey + "_child", rotateModel);
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
        Dropped = false;
        hasTriggered = false;
    }
}
