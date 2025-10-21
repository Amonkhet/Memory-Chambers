using UnityEngine;
using UnityEngine.InputSystem;

public class BlueprintActivate : MonoBehaviour
{
    // Attributes
    
    [Header("Corresponding building ID")]
    [SerializeField] string buildingID;
    
    // Get if the current blueprint tile is occupied by correct building id
    public bool IsOccupied { get; private set; }
    public string currentBuildingID { get; private set; }

    // When building is on this blueprint tile
    void BuildingOnTile(Collider collider)
    {
        var value = collider.GetComponentInParent<BuildingKeyID>();
        if (!value)
        {
            return;
        }
        if (!IsOccupied && (string.IsNullOrEmpty(buildingID) || buildingID == value.BuildingID))
        {
            IsOccupied = true;
            currentBuildingID = value.BuildingID;
        }
    }
    // Reset status
    void BuildingOffTile(Collider collider)
    {
        var value = collider.GetComponentInParent<BuildingKeyID>();
        if (!value)
        {
            return;
        }
        if(!IsOccupied && value.BuildingID == currentBuildingID)
        {
            IsOccupied = false;
            currentBuildingID = null;
        }
    }
    // Check if building on correct tile
    public bool IsBuildingOnCorrectTile()
    {
        return IsOccupied && currentBuildingID == buildingID;
    }
}
