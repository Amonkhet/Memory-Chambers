using UnityEngine;

public class BuildingRiseSave : MonoBehaviour
{
    [Header("Building key id")] 
    [SerializeField] private string buildingKey;
    
    [Header("Building stage")]
    [SerializeField] private Transform artRoot;       
    [SerializeField] private GameObject[] stageModels;
    
    [SerializeField] private int currentStage = 0;
    void Awake()
    {
        if (!artRoot)
        {
            var t = transform.Find("Art");
            if (t) artRoot = t;
        }

        if ((stageModels == null || stageModels.Length == 0) && artRoot)
        {
            stageModels = new GameObject[4];
            stageModels[0] = artRoot.Find("Building_1D")?.gameObject;
            stageModels[1] = artRoot.Find("Building_2D")?.gameObject;
            stageModels[2] = artRoot.Find("Building_3D")?.gameObject;
            stageModels[3] = artRoot.Find("Building_4D")?.gameObject;
        }
    }

    void Start()
    {
        // Check if there is a saved stage
        if (GameStatus.GetBuildingRiseStage(buildingKey, out var saved))
        {
            currentStage = Mathf.Clamp(saved, 0, stageModels.Length - 1);
            SaveBuildingStage();                  
            return;
        }
        // Get current stage
        if (TryGetActiveStage(out var activeStage))
        {
            currentStage = activeStage;
            GameStatus.SaveBuildingRiseStage(buildingKey, currentStage); 
            return;
        }
        
        

    }
    void LateUpdate()
    {
        if (TryGetActiveStage(out var activeStage) && activeStage != currentStage)
        {
            currentStage = activeStage;
            GameStatus.SaveBuildingRiseStage(buildingKey, currentStage);
        }
    }


    void SaveBuildingStage()
    {
        if (stageModels == null) return;
        for (int i = 0; i < stageModels.Length; i++)
            if (stageModels[i]) stageModels[i].SetActive(i == currentStage);

        GameStatus.SaveBuildingRiseStage(buildingKey, currentStage); 
    }
    // Change stage int to save building stage status
    public void IncrementStage(int stage)
    {
        currentStage = Mathf.Clamp(currentStage + stage, 0, stageModels.Length - 1);
        SaveBuildingStage();
    }

    bool TryGetActiveStage(out int stage)
    {
        stage = -1;
        if (stageModels == null)
        {
            return false;
        }
        for (int i = 0; i < stageModels.Length; i++)
        {
            var go = stageModels[i];
            if (go && go.activeSelf) { stage = i; return true; }
        }
        return false;
    }
    
}

