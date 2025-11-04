using UnityEngine;

public class UITextPageController : MonoBehaviour
{
    public GameObject textPagePanel; // Assign your TextPagePanel in the Inspector

    // Called when the "Open" button is clicked
    public void ShowTextPage()
    {
        textPagePanel.SetActive(true);
    }

    // Called when the "Close" button is clicked
    public void HideTextPage()
    {
        textPagePanel.SetActive(false);
    }
}
