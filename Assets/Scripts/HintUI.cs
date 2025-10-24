using UnityEngine;
using TMPro;
public class HintUI : MonoBehaviour
{
    // Show hint message
    public TMP_Text message;
    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        HintManager.OnDisplayHint += ShowHintUI;
        HintManager.OnClearHint   += HideHintUI;
    }
    void ShowHintUI(string text)
    {
        if (message) message.text = text;
        group.alpha = 1f;  
    }

    void HideHintUI()
    {
        group.alpha = 0f; 
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
