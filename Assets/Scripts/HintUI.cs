using UnityEngine;
using TMPro;
using System.Collections;
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

        if (message)
        {
            message.text = text;
        }
        StopAllCoroutines();
        group.alpha = 1f;
        StartCoroutine(AutoHideHint());
    }

    // Wait a bit and fade
    IEnumerator AutoHideHint()
    {
        yield return new WaitForSeconds(1f);
        float fadeTime = 0.5f;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / fadeTime;
            group.alpha = Mathf.Lerp(1f, 0f, time);
            yield return null;
        }
        group.alpha = 0f;
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
