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
    public string currentId { get; private set; } 
    // Broadcast this agent scale/type
    public static System.Action<NavMeshAgent, Transform> OnPlayerSwitched;
    // Save and allow get by others
    public static NavMeshAgent CurrentAgent
    {
        get; private set;
    }

    public static Transform CurrentPlayerRoot
    {
        get; private set;
    }
    // Switch player function
    public void SwitchPlayer(string id)
    {
        currentId = id;
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
        //Update agent scale to event
        CurrentAgent = agent;
        CurrentPlayerRoot = this.transform;
        OnPlayerSwitched?.Invoke(agent, this.transform);
        // Tell Cinemachine which camera to use
        SwitchCinemachineCamera(id);
    }
    private void SwitchCinemachineCamera(string id)
    {
        // Find three vritual camera
        var camXS = GameObject.Find("VCXS");
        var camS  = GameObject.Find("VCS");
        var camL  = GameObject.Find("VCL");

        // Set camera priority to default 5
        if (camXS) camXS.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 5;
        if (camS)  camS.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 5;
        if (camL)  camL.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 5;

        // Change camera priority based on player id
        if (id == "XS" && camXS)
            camXS.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 20;
        else if (id == "S" && camS)
            camS.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 20;
        else if (id == "L" && camL)
            camL.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 20;
    }

    // void Update()
    // {
    //      if (Input.GetKeyDown(KeyCode.Alpha1))
    //          SwitchPlayer("XS");   
    //      if (Input.GetKeyDown(KeyCode.Alpha2))
    //          SwitchPlayer("S");    
    //      if (Input.GetKeyDown(KeyCode.Alpha3))
    //          SwitchPlayer("L");   
    // }
}
