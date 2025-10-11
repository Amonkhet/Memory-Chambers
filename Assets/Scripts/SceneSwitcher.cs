using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitcher : MonoBehaviour
{
    // Event listner
    void OnEnable()
    {
        EventManager.OnDoorEnter += ActivateDoor;
    }

    void OnDisable()
    {
        EventManager.OnDoorEnter -= ActivateDoor;
    }
    // Listen to event
    void ActivateDoor(EventManager.DoorToScene door)
    {
        SceneSwitchData.SetData(door.targetScene, door.spawnPoint, door.scale);
        SceneManager.LoadScene(door.targetScene);
    }
}
