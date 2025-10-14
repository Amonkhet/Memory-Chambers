using UnityEngine;
using System.Collections.Generic;
public class GameStatus : MonoBehaviour
{
    // Save game status
    private static GameStatus _instance;
    // Save mechanics status with key id
    private static readonly Dictionary<string, bool> activatedMechanics = new Dictionary<string, bool>();
    // Add struct to save the dropped book status
    public struct BookDroppedStatus
    {
        public Vector3 droppedPosition;
        public Vector3 droppedRotation;
        public Vector3 droppedScale;
    }
    private static readonly Dictionary<string, BookDroppedStatus> droppedStatus = new Dictionary<string, BookDroppedStatus>();
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
        droppedStatus.Clear();
    }
    // Save the dropped book position
    public static void SaveBookDroppedStatus(string key, Transform transform)
    {
        // Save vector position
        droppedStatus[key] = new BookDroppedStatus
        {
            droppedPosition = transform.localPosition,
            droppedRotation = transform.localEulerAngles,
            droppedScale = transform.localScale
        };
    }

    public static bool GetBookDroppedStatus(string key, out BookDroppedStatus value)
    {
        return droppedStatus.TryGetValue(key, out value);
    }
}
