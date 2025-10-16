using UnityEngine;
using UnityEngine.AI;
public class PlayerSwitcher : MonoBehaviour
{
    // Input attributes
    public Transform playerRoot;
    public Transform agentsRoot;
    public Transform cameraRoot;
    public CapsuleCollider capsuleCollider;
    public NavMeshAgent agent;

    private GameObject currentPlayer;
    // Switch player function
    public void SwitchPlayer(string id)
    {
        agent.isStopped = true;
        agent.ResetPath();
        // Delete previous player model
        if(currentPlayer) Destroy(currentPlayer);
        // Find nav mesh agent
        var agentTransform = agentsRoot.Find(id + "Player");
        var agentTemplate = agentTransform.GetComponent<NavMeshAgent>();
        // Find player model
        var playerTransform = playerRoot.Find(id + "Model");
        currentPlayer = Instantiate(playerTransform.gameObject, playerRoot);
        currentPlayer.SetActive(true);
        currentPlayer.transform.localPosition = Vector3.zero;
        currentPlayer.transform.localRotation = Quaternion.identity;
        currentPlayer.transform.localScale    = playerTransform.localScale;
        // Sync agent and model collider with capsulemodel
        capsuleCollider.height = agentTemplate.height;
        capsuleCollider.radius = agentTemplate.radius;
        capsuleCollider.center = new Vector3(0, agentTemplate.height / 2, 0);
        // Sync agent and model collider attributes(will be changed to model later)
        capsuleCollider.height = agentTemplate.height;
        capsuleCollider.radius = agentTemplate.radius;
        capsuleCollider.center = new Vector3(0, agentTemplate.height * 0.5f, 0);
        agent.agentTypeID = agentTemplate.agentTypeID;
        agent.radius = agentTemplate.radius;
        agent.height = agentTemplate.height;
        agent.speed = agentTemplate.speed;
        agent.acceleration = agentTemplate.acceleration;
        agent.angularSpeed = agentTemplate.angularSpeed;
        agent.areaMask = agentTemplate.areaMask;
        agent.baseOffset = agentTemplate.baseOffset;
        // Reset agent position
        agent.Warp(transform.position);
        agent.isStopped = false;
        // Set camera
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchPlayer("XS");   // 按键 1 切换为 XS 体型
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchPlayer("S");    // 按键 2 切换为 S 体型
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchPlayer("L");    // 按键 3 切换为 L 体型
    }
}
