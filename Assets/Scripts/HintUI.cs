using UnityEngine;
using TMPro;
public class HintUI : MonoBehaviour
{
    // Show hint message
    public TMP_Text message;
    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>() ?? GetComponentInParent<CanvasGroup>();
        if (group)
        {
            group.alpha = 0f;
        }
    }
    void ShowHintUI(string text)
    {
        if (!group)
        {
            return;
        }
        if (message) message.text = text;
        group.alpha = 1f;  
    }

    void HideHintUI()
    {
        if (!group)
        {
            return;
        }
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
    void OnDestroy()
    {
        HintManager.OnDisplayHint -= ShowHintUI;
        HintManager.OnClearHint   -= HideHintUI;
    }
}
