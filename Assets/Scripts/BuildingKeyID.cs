using UnityEngine;

public class BuildingKeyID : MonoBehaviour
{
    // This used to mark building with unique key id
    [Header("Unique Key Id for each building")]
    [SerializeField] private string buildingID;
    // Getter to get this key
    public string BuildingID
    {
        get => buildingID;
    }
}
