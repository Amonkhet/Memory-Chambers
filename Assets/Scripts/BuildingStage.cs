using UnityEngine;

public class BuildingStage : MonoBehaviour
{
    [Header("Building Stage")]
    [SerializeField] int buildingStage = 1;
    public bool IsBuildingFirst => buildingStage <= 1;
    public bool IsBuildingLast  => buildingStage >= 4;

    public void SetBuildingStage(int stage)
    {
        buildingStage = Mathf.Clamp(stage, 1, 4);
    }
}
