using UnityEngine;
using UnityEngine.AI;

public class SyncPlayerAnimation : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    [SerializeField] float damp = 0.1f;

    void Reset() {
        if (!animator) animator = GetComponent<Animator>();
        if (!agent) agent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent || !animator) return;
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed, damp, Time.deltaTime);
        animator.SetBool("Moving", speed > 0.05f);
    }
}
