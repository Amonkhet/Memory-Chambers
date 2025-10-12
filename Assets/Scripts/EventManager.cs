using System;
using UnityEngine;

public static class EventManager
{
    // Use a struct to pack switch scene data
    public struct DoorToScene
    {
        public string targetScene;
        public string spawnPoint;
        public string scale;

        public DoorToScene(string targetScene, string spawnPoint, string scale)
        {
            this.targetScene = targetScene;
            this.spawnPoint = spawnPoint;
            this.scale = scale;
        }
    }
    // Event 
    public static event Action<DoorToScene> OnDoorEnter;
    public static void WhenEnterDoor(DoorToScene door) => OnDoorEnter?.Invoke(door);
    // Interactive Event
    public static event Action<RaycastHit, Transform> OnObjectClicked;
    public static void WhenObjectClicked(RaycastHit hit, Transform player) => OnObjectClicked?.Invoke(hit, player);
}
