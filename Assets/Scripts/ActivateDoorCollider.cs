using UnityEngine;

public class ActivateDoorCollider : MonoBehaviour
{
    [Header("Mechanic Key same as open door")]
    [SerializeField] private string mechanicKey = "DoorLActivate";

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }
        GameStatus.SetDoorLState(mechanicKey, true);
    }
}
