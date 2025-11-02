using System;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Mechanics key id")]
    [SerializeField] private string mechanicKey = "DoorLActivate";
    [SerializeField] private bool activeWhenDropped = true;
    [Header("Player")]
    [SerializeField] private string playerTag = "Player";
    [Header("Active when collider triggered")]
    [SerializeField] private bool activeWhenTriggered = true;
    private bool triggerOnce = true;
    bool hasTriggeredOnce = false;
    void Awake()
    {
        UpdateState();
    }

    void OnEnable()
    {
        UpdateState();
    }

    void Start()
    {
        UpdateState();
    }
    
    private void UpdateState()
    {
        bool triggered = GameStatus.GetDoorLState(mechanicKey);
        bool targetActive = activeWhenTriggered ? triggered : !triggered;
        gameObject.SetActive(targetActive);
    }
}
