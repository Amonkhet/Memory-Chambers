using UnityEditor.PackageManager;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Store attribute across scene
    public static bool hasData;
    public static string targetScene;
    public static string spawnPoint;
    public static string scaleId;

    public static void SetData(string scene, string spawn, string scale)
    {
        hasData = true;
        targetScene = scene;
        spawnPoint = spawn;
        scaleId = scale;
    }

    public static void ClearData()
    {
        hasData = false;
    }
}
