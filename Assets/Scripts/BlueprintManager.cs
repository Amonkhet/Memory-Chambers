using UnityEngine;
using UnityEngine.Playables;

public class BlueprintManager : MonoBehaviour
{
    // Attribute
    [Header("Blueprint tile")] 
    [SerializeField] private BlueprintActivate blueprintB;
    [SerializeField] private BlueprintActivate blueprintC;
    [SerializeField] private BlueprintActivate blueprintD;
    [Header("Endscene timeline")]
    public PlayableDirector  timeline;  
    
    public bool AllOnCorrectTile {get; private set;}
    // Only check if all on correct tile if there is a difference between last state and current state
    private bool lastState;
    private static bool hasPlayed = false;
    // Update is called once per frame
    void Update()
    {
        if (hasPlayed)
        {
            return;
        }
        AllOnCorrectTile = CheckAllOnCorrectTile();
        if (AllOnCorrectTile != lastState)
        {
            lastState = AllOnCorrectTile;
            if (AllOnCorrectTile && !hasPlayed)
            {
                Debug.Log("All On Correct Tile 1");
                WhenAllOnCorrectTile();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            WhenAllOnCorrectTile();
        }
    }

    bool CheckAllOnCorrectTile()
    {
        return blueprintB && blueprintB.IsBuildingOnCorrectTile() && blueprintC && blueprintC.IsBuildingOnCorrectTile() && blueprintD && blueprintD.IsBuildingOnCorrectTile();
    }
    
    // When all on correct tile, use this method
    void WhenAllOnCorrectTile()
    {
        timeline.Play();
        Debug.Log("All On Correct Tile 2");
        hasPlayed = true;
    }
    void OnEnable()
    {
        if (timeline) timeline.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        if (timeline) timeline.stopped -= OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        director.gameObject.SetActive(false); 
    }

}
