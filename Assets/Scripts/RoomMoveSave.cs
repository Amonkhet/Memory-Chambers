using UnityEngine;

public class RoomMoveSave : MonoBehaviour
{
    // Save room move transform to game status
    private string roomID = "RoomLife";
    [SerializeField] private float saveEPS = 0.001f;  
    [SerializeField] private float saveInterval = 0.05f;
    private Vector3 lastSavedPosition;
    private float lastSaveTime;

    void Start()
    {
        // Using the same logic as building save
        if (GameStatus.GetRoomMoveStatus(roomID, out var savedPos))
        {
            transform.position = savedPos;
            lastSavedPosition = savedPos;
        }
        else
        {
            GameStatus.SaveRoomMoveStatus(roomID, transform.position);
            lastSavedPosition = transform.position;
        }
        lastSaveTime = Time.time;
    }

    void LateUpdate()
    {
        if (Time.time - lastSaveTime < saveInterval)
        {
            return;
        }
        if ((transform.position - lastSavedPosition).sqrMagnitude > saveEPS * saveEPS)
        {
            GameStatus.SaveRoomMoveStatus(roomID, transform.position);
            lastSavedPosition = transform.position;
            lastSaveTime = Time.time;
        }
    }
    private void OnDisable()
    {
        GameStatus.SaveRoomMoveStatus(roomID, transform.position);
    }

    private void OnDestroy()
    {
        GameStatus.SaveRoomMoveStatus(roomID, transform.position);
    }
}
