using UnityEngine;

public class ColliderHint : MonoBehaviour
{
    [Header("Collider hint text")]
    [SerializeField] private string text;
    bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;  
            Hint.ShowHint(text);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTriggered = false;  
            Hint.HideHint();
        }
    }
}
