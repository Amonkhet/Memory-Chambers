using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetWorkScene : MonoBehaviour
{
    // Keybinds
    [Header("Key binds")]
    [SerializeField] KeyCode resetKey = KeyCode.R;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            GameStatus.ClearBuildingPositionStatus();
            // Reset all building position to initial state
            var buildings = FindObjectsOfType<BuildingMove>();
            foreach (var building in buildings)
            {
                building.ResetBuildingPosition();
            }
            // var currentScene = SceneManager.GetActiveScene();
            // SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
