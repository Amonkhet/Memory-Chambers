using UnityEngine;

public static class SceneSwitchData
{
    // Store attribute across scene
    public static bool HasData;
    public static string TargetScene;
    public static string SpawnPoint;
    public static string ScaleId;

    public static void SetData(string scene, string spawn, string scale)
    {
        HasData = true;
        TargetScene = scene;
        SpawnPoint = spawn;
        ScaleId = scale;
    }

    public static void ClearData()
    {
        HasData = false;
    }
}
