using UnityEngine;
using System.Collections.Generic;
public static class GameStatus
{
    // Save mechanics status with key id
    private static Dictionary<string, bool> activatedMechanics = new Dictionary<string, bool>();
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
