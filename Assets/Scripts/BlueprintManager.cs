using UnityEngine;

public class BlueprintManager : MonoBehaviour
{
    // Attribute
    [Header("Blueprint tile")] 
    [SerializeField] private BlueprintActivate blueprintA;
    [SerializeField] private BlueprintActivate blueprintB;
    [SerializeField] private BlueprintActivate blueprintC;
    [SerializeField] private BlueprintActivate blueprintD;
    
    public bool AllOnCorrectTile {get; private set;}
    // Only check if all on correct tile if there is a difference between last state and current state
    private bool lastState;

    // Update is called once per frame
    void Update()
    {
        AllOnCorrectTile = CheckAllOnCorrectTile();
        if (AllOnCorrectTile != lastState)
        {
            lastState = AllOnCorrectTile;
            if (AllOnCorrectTile)
            {
                Debug.Log("All On Correct Tile 1");
                WhenAllOnCorrectTile();
            }
        }
    }

    bool CheckAllOnCorrectTile()
    {
        return blueprintA && blueprintA.IsBuildingOnCorrectTile() && blueprintB && blueprintB.IsBuildingOnCorrectTile() && blueprintC && blueprintC.IsBuildingOnCorrectTile() && blueprintD && blueprintD.IsBuildingOnCorrectTile();
    }
    
    // When all on correct tile, use this method
    void WhenAllOnCorrectTile()
    {
        Debug.Log("All On Correct Tile 2");
    }
}
