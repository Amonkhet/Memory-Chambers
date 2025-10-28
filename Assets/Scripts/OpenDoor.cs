using System;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Mechanics key id")]
    [SerializeField] private string mechanicKey = "BookDrop";
    [SerializeField] private bool activeWhenDropped = true;

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
        bool dropped = GameStatus.IsActivated(mechanicKey);
        bool targetActive = activeWhenDropped ? dropped : !dropped;
        gameObject.SetActive(targetActive);
    }
}
