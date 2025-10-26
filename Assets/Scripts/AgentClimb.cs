using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AgentClimb : MonoBehaviour
{
    public Animator animator;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;  // 禁止 NavMeshAgent 自动处理 OffMeshLink
    }

    IEnumerator Start()
    {
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                animator.SetBool("isClimbing", true);
                animator.SetFloat("Speed", 0f); 

                var data = agent.currentOffMeshLinkData;
                var start = data.startPos;
                var end = data.endPos;
                float t = 0f, dur = 1.2f;

                while (t < 1f)
                {
                    t += Time.deltaTime / dur;
                    transform.position = Vector3.Lerp(start, end, t);
                    yield return null;
                }

                agent.CompleteOffMeshLink();  
                animator.SetBool("isClimbing", false); 
                //animator.SetFloat("Speed", 1f);  

            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            //start climbing animation
            animator.SetBool("isClimbing", true);
            animator.SetFloat("Speed", 0f);  
            //agent.enabled = false;  
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            // stop climbing animation
            animator.SetBool("isClimbing", false);
            animator.SetFloat("Speed", 1f);  // walking
            // agent.enabled = true;  // 
        }
    }
}
