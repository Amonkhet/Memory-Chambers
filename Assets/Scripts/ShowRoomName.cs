using UnityEngine;
using TMPro;

public class ShowRoomName : MonoBehaviour
{
    public string roomName = "Outworld";
    public TextMeshProUGUI areaText;
    public float displayTime = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowText());
        }
    }

    private System.Collections.IEnumerator ShowText()
    {
        areaText.text = "Entered: " + roomName;
        areaText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        areaText.gameObject.SetActive(false);
    }
}
