using UnityEngine;
using TMPro;
public class HintUI : MonoBehaviour
{
    // Show hint message
    public TMP_Text message;

    void ShowHintUI(string text)
    {
        message.text = text;
        gameObject.SetActive(true);
    }

    void HideHintUI()
    {
        gameObject.SetActive(false);
    }
    
    void OnEnable()
    {
        HintManager.OnDisplayHint += ShowHintUI;
        HintManager.OnClearHint += HideHintUI;
    }

    void OnDisable()
    {
        HintManager.OnDisplayHint -= ShowHintUI;
        HintManager.OnClearHint -= HideHintUI;
    }
}
