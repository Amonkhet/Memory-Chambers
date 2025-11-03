using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    // Play game button
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("GuideScene");
    }
}
