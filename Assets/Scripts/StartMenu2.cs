using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu2 : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("RoomLifeF");
    }
}
