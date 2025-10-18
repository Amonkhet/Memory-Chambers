using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;

public enum BuildingSceneStage { S0, S1, S2 }   
public enum DoorTiggerID    { A, B, C, D }     

public class BuildingTimelineManager : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] PlayableDirector timelineForward;
    [SerializeField] PlayableDirector timelineBackward;
    [SerializeField] PlayableDirector timelineReset;

    [Header("Door trigger")]
    [SerializeField] GameObject doorA; 
    [SerializeField] GameObject doorD; 
    [SerializeField] GameObject doorB; 
    [SerializeField] GameObject doorC; 

    [Header("Player")]
    [SerializeField] NavMeshAgent playerAgent;            
    [SerializeField] MonoBehaviour playerControlScript;   
    [SerializeField] Transform doorGroup1ExitPoint;         
    [SerializeField] Transform doorGroup2ExitPoint;        

    [Header("Current building scene stage")]
    [SerializeField] BuildingSceneStage currentBuildingStge = BuildingSceneStage.S1;

    // Save status while playing
    private bool isPlaying;
    private bool playerControlEnabled;
    private bool playerPosUpdate;
    private bool playerRotUpdate;
        

    void Awake()
    {
        if (timelineForward)  timelineForward.stopped  += _ => OnTimelineStopped();
        if (timelineBackward) timelineBackward.stopped += _ => OnTimelineStopped();
        if (timelineReset) timelineReset.stopped += _ => OnTimelineStopped();
        SwitchDoorWithStage(currentBuildingStge);
    }

    // Activate door
    public void ActivateDoor(DoorTiggerID door)
    {
        if (isPlaying)
        {
            return;
        }

        switch (currentBuildingStge)
        {
            case BuildingSceneStage.S1:
                if (door == DoorTiggerID.A)
                {
                    // S1 with A to S2
                    PlayForward(BuildingSceneStage.S2, () =>
                    {
                        SetGroup2Door(DoorTiggerID.C);   
                        MovePlayerTo(doorGroup2ExitPoint); 
                    });
                }
                else if (door == DoorTiggerID.B)
                {
                    // S1 with B to S0
                    PlayReset(BuildingSceneStage.S0, () =>
                    {
                        SetGroup1Door(DoorTiggerID.D);    
                        MovePlayerTo(doorGroup1ExitPoint); 
                    });
                }
                break;

            case BuildingSceneStage.S2:
                if (door == DoorTiggerID.C)
                {
                    // S2 with C to S1
                    PlayBackward(BuildingSceneStage.S1, () =>
                    {
                        SetGroup2Door(DoorTiggerID.B);   
                        MovePlayerTo(doorGroup1ExitPoint); 
                    });
                }
                break;

            case BuildingSceneStage.S0:
                if (door == DoorTiggerID.D)
                {
                    // S0 with D to S1 palyforward
                    PlayForward(BuildingSceneStage.S1, () =>
                    {
                        SetGroup1Door(DoorTiggerID.A);    
                        MovePlayerTo(doorGroup2ExitPoint); 
                    });
                }
                break;
        }
    }
    // Stop all playable directors
    void StopAllDirectors()
    {
        if (timelineForward  && timelineForward.state  == PlayState.Playing) timelineForward.Stop();
        if (timelineBackward && timelineBackward.state == PlayState.Playing) timelineBackward.Stop();
        if (timelineReset && timelineReset.state == PlayState.Playing) timelineReset.Stop();
    }

    // Timeline playforward and backward

    void PlayForward(BuildingSceneStage toState, System.Action after)
    {
        StopAllDirectors();
        if (!timelineForward)
        {
            return;
        }
        ActivateTimelineAnimation();
        
        timelineForward.time = 0.001;
        timelineForward.Evaluate();
        timelineForward.Play();

        currentBuildingStge = toState;
        StartCoroutine(InvokeOnStop(timelineForward, after));
    }

    void PlayBackward(BuildingSceneStage toState, System.Action after)
    {
        StopAllDirectors();
        if (!timelineBackward)
        {
            return;
        }
        ActivateTimelineAnimation();
        
        timelineBackward.time = Mathf.Max(0.001f, (float)timelineBackward.duration - 0.001f);
        timelineBackward.Evaluate();
        timelineBackward.Play();


        var root = timelineBackward.playableGraph.GetRootPlayable(0);
        if (root.IsValid())
        {
            root.SetSpeed(-1.0);
        }

        currentBuildingStge = toState;
        StartCoroutine(InvokeOnStop(timelineBackward, after));
    }
    
    void PlayReset(BuildingSceneStage toState, System.Action after)
    {
        if (!timelineReset)
        {
            return;
        }

        StopAllDirectors();
        ActivateTimelineAnimation();

        const double EPS = 0.001; 
        timelineReset.time = Mathf.Max((float)timelineReset.duration - (float)EPS, (float)EPS);
        timelineReset.Evaluate();

        var root = timelineReset.playableGraph.GetRootPlayable(0);
        if (root.IsValid())
        {
            root.SetSpeed(-1.0);
        } 
        timelineReset.Play();
        currentBuildingStge = toState;
        StartCoroutine(InvokeOnStop(timelineReset, after));
    }

    void ActivateTimelineAnimation()
    {
        isPlaying = true;
        // Deactivate door while playing
        SetAllDoorsActive(false);

        // Deactivate playr movement
        if (playerControlScript)
        {
            playerControlEnabled = playerControlScript.enabled;
            playerControlScript.enabled = false;
        }
        if (playerAgent)
        {
            playerPosUpdate = playerAgent.updatePosition;
            playerRotUpdate = playerAgent.updateRotation;
            playerAgent.isStopped = true;
            playerAgent.updatePosition = false;
            playerAgent.updateRotation = false;
            playerAgent.ResetPath();
        }
    }
    void FinishPlay()
    {
        // Recover player
        if (playerControlScript) playerControlScript.enabled = playerControlEnabled;
        if (playerAgent)
        {
            playerAgent.updatePosition = playerPosUpdate;
            playerAgent.updateRotation = playerRotUpdate;
            playerAgent.isStopped = false;
        }
        SwitchDoorWithStage(currentBuildingStge);
        isPlaying = false;
    }
    void OnTimelineStopped()
    {
        FinishPlay();
    }

    System.Collections.IEnumerator InvokeOnStop(PlayableDirector d, System.Action after)
    {
        // wait until stop
        yield return null;
        while (d && d.state == PlayState.Playing) yield return null;
        after?.Invoke();
        FinishPlay();
    }
    

    void SwitchDoorWithStage(BuildingSceneStage s)
    {
        switch (s)
        {
            case BuildingSceneStage.S1:
                SetGroup1Door(DoorTiggerID.A);
                SetGroup2Door(DoorTiggerID.B);
                break;

            case BuildingSceneStage.S2:
                SetGroup1DoorActive(false);
                SetGroup2Door(DoorTiggerID.C);
                break;

            case BuildingSceneStage.S0:
                SetGroup2DoorActive(false);
                SetGroup1Door(DoorTiggerID.D);
                break;
        }
    }

    void SetGroup1Door(DoorTiggerID id) => SetPair(doorA, doorD, id == DoorTiggerID.A);
    void SetGroup2Door(DoorTiggerID id) => SetPair(doorB, doorC, id == DoorTiggerID.B);

    void SetPair(GameObject main, GameObject alternative, bool mainOn)
    {
        if (main)
        {
            main.SetActive(mainOn);
        }

        if (alternative)
        {
            alternative.SetActive(!mainOn);
        }
    }

    void SetGroup1DoorActive(bool on)
    {
        if (doorA) doorA.SetActive(on && currentBuildingStge == BuildingSceneStage.S1);
        if (doorD) doorD.SetActive(on && currentBuildingStge == BuildingSceneStage.S0);
    }

    void SetGroup2DoorActive(bool on)
    {
        if (doorB) doorB.SetActive(on && currentBuildingStge == BuildingSceneStage.S1);
        if (doorC) doorC.SetActive(on && currentBuildingStge == BuildingSceneStage.S2);
    }

    void SetAllDoorsActive(bool on)
    {
        if (doorA) doorA.SetActive(on);
        if (doorB) doorB.SetActive(on);
        if (doorC) doorC.SetActive(on);
        if (doorD) doorD.SetActive(on);
    }
    
    // Move player to another door when animation stopped
    void MovePlayerTo(Transform transform)
    {
        if (!playerAgent || !transform)
        {
            return;
        }
        playerAgent.Warp(transform.position);
        playerAgent.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }
}