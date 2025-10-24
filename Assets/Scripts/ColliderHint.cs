using UnityEngine;

public class ColliderHint : MonoBehaviour
{
    [Header("Collider hint text")]
    [SerializeField] private string text;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hint.ShowHint(text);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hint.HideHint();
        }
    }
}
