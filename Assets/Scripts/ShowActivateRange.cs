using UnityEngine;

public class ShowActivateRange : MonoBehaviour
{
    // Show activate range visually
    [SerializeField] float activateRange;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.7f);
        Gizmos.DrawWireSphere(transform.position, activateRange);
    }
}
