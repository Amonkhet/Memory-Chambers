using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // NavMeshAgent 和 Animator 引用
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Control Settings")]
    [SerializeField] float rangeWalkable = 0.5f;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 10f;

    // 爬梯相关变量
    [Header("Climb Settings")]
    [SerializeField] float climbSpeed = 1.0f;
    [SerializeField] float climbDuration = 3.5f;
    private bool isClimbing = false;

    private bool isJumping = false;

    // Start 方法
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    // Update 方法
    void Update()
    {
        // 检查鼠标点击事件
        if (Input.GetMouseButtonDown(0) && !isJumping && !isClimbing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitClick))
            {
                EventManager.WhenObjectClicked(hitClick, transform);
            }

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                int mask = agent.areaMask;
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, rangeWalkable, mask))
                {
                    agent.SetDestination(navHit.position);
                }
                else
                {
                    Debug.Log("Clicked point is not a walkable area.");
                }
            }
        }

        // 爬梯子检测
        if (agent.isOnOffMeshLink && !isClimbing)
        {
            StartCoroutine(HandleClimb());
        }

        // 如果正在爬梯子，则停止行走动画
        if (isClimbing)
        {
            anim.SetBool("isClimbing", true);
            anim.SetFloat("Speed", 0f); // 停止行走动画
        }
        else
        {
            // 设置 Speed 动画参数（当不爬梯时，基于速度设置行走动画）
            float normalizedSpeed = Mathf.InverseLerp(0f, agent.speed, agent.velocity.magnitude);
            anim.SetFloat("Speed", normalizedSpeed);
        }
    }

    // 处理爬梯子逻辑
    private IEnumerator HandleClimb()
    {
        isClimbing = true;
        anim.SetBool("isClimbing", true); // 设置爬梯动画参数

        agent.isStopped = true; // 停止 NavMeshAgent 的自动移动
        Vector3 climbStartPos = transform.position; // 确保爬升从当前位置开始
        Vector3 climbEndPos = agent.currentOffMeshLinkData.endPos; // 获取 OffMeshLink 的目标位置
        float climbTime = 0f;

        // 以爬梯速度过渡到目标位置
        while (climbTime < climbDuration)
        {
            climbTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(climbTime / climbDuration);
            transform.position = Vector3.Lerp(climbStartPos, climbEndPos, lerpFactor);
            yield return null;
        }

        agent.CompleteOffMeshLink(); // 确保完成链接
        isClimbing = false;
        anim.SetBool("isClimbing", false); // 动画回到正常状态
        agent.isStopped = false; // 恢复 NavMeshAgent 的移动功能
    }
}
