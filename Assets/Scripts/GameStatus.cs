using UnityEngine;
using System.Collections.Generic;
public class GameStatus : MonoBehaviour
{
    // Save game status
    private static GameStatus _instance;
    // Save mechanics status with key id
    private static readonly Dictionary<string, bool> activatedMechanics = new Dictionary<string, bool>();
    // For Book Drop Status: Add struct to save the dropped book status
    public struct BookDroppedStatus
    {
        public Vector3 droppedPosition;
        public Vector3 droppedRotation;
        public Vector3 droppedScale;
    }
    private static readonly Dictionary<string, BookDroppedStatus> droppedStatus = new Dictionary<string, BookDroppedStatus>();
    
    // For Move building Status:
    private static readonly Dictionary<string, Vector3> buildingPosition = new Dictionary<string, Vector3>();
    
    // For building rise status
    private static readonly Dictionary<string, int> buildingRiseStage = new Dictionary<string, int>();
    
    // For move room status in outworld scene
    private static readonly Dictionary<string, Vector3> roomMove = new Dictionary<string, Vector3>();
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
    // For book drop status in room life
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
        buildingPosition.Clear();
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
    
    // For move building status in room work
    public static void SaveBuildingPositionStatus(string key, Vector3 position)
    {
        buildingPosition[key] = position;
    }

    public static bool GetBuildingPositionStatus(string key, out Vector3 position)
    {
        return buildingPosition.TryGetValue(key, out position);
    }
    // Reset building status
    public static void ClearBuildingPositionStatus()
    {
        buildingPosition.Clear();
    }
    
    // For room move in outworld scene
    public static void SaveRoomMoveStatus(string key, Vector3 position)
    {
        roomMove[key] = position;
    }

    public static bool GetRoomMoveStatus(string key, out Vector3 position)
    {
        return roomMove.TryGetValue(key, out position);
    }
    // Reset building status
    public static void ClearRoomMoveStatus()
    {
        roomMove.Clear();
    }
    
    // For building rise status in work scene
    public static void SaveBuildingRiseStage(string key, int stage)
    {
        buildingRiseStage[key] = stage;
    }

    public static bool GetBuildingRiseStage(string key, out int stage)
    {
        return buildingRiseStage.TryGetValue(key, out stage);
    }
}
