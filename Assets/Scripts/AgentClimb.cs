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
                // 开始爬梯动画
                animator.SetBool("isClimbing", true);
                // 禁用行走动画
                animator.SetFloat("Speed", 0f); 

                var data = agent.currentOffMeshLinkData;
                var start = data.startPos;
                var end = data.endPos;
                float t = 0f, dur = 1.2f;

                // 移动到目标位置
                while (t < 1f)
                {
                    t += Time.deltaTime / dur;
                    transform.position = Vector3.Lerp(start, end, t);
                    yield return null;
                }

                agent.CompleteOffMeshLink();  // 完成 OffMeshLink
                animator.SetBool("isClimbing", false);  // 停止爬梯动画
                animator.SetFloat("Speed", 1f);  // 恢复行走动画（如果需要的话）

            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            // 开始爬梯动画
            animator.SetBool("isClimbing", true);
            animator.SetFloat("Speed", 0f);  // 停止行走动画
            agent.enabled = false;  // 禁用 NavMeshAgent 的控制
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            // 停止爬梯动画
            animator.SetBool("isClimbing", false);
            animator.SetFloat("Speed", 1f);  // 恢复行走动画
            agent.enabled = true;  // 恢复 NavMeshAgent 控制
        }
    }
}
