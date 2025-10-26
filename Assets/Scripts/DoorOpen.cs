using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;        // degrees to rotate
    public float openSpeed = 2f;         // how fast it rotates open/close

    private Quaternion closedRot;
    private Quaternion openRot;
    private bool isOpen = false;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        Quaternion targetRot = isOpen ? openRot : closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * openSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;
        }
    }
}

