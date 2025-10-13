using UnityEngine;
using System.Collections.Generic;
public class GameStatus : MonoBehaviour
{
    // Save game status
    private static GameStatus _instance;
    // Save mechanics status with key id
    private static Dictionary<string, bool> activatedMechanics = new Dictionary<string, bool>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Record mechanics status
    public static void Activated(string key, bool value = true)
    {
        activatedMechanics[key] = value;
    }
    // Find mechanics status
    public static bool IsActivated(string key)
    {
        return activatedMechanics.TryGetValue(key, out bool values) && values;
    }
    // Clear all status
    public static void ClearStatus()
    {
        activatedMechanics.Clear();
    }
}
