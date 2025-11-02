using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSubtitle : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup panelGroup;
    public TMP_Text subtitleText;

    [Header("Subtitle Settings")]
    [TextArea(5, 10)]
    public string[] lines = {
        "Where... where am I? This place... it’s my room.",
        "The cabinet, the bed, the shelves — everything looks so familiar, yet so distant.",
        "Why am I so small? Have I shrunk, or has this world grown around me?",
        "I feel trapped inside my shell.",
        "I need to get out... I can’t stay here any longer."
    };

    [Header("Timing Settings")]
    public float typingSpeed = 0.06f;
    public float lineDelay = 1.6f;
    public float fadeTime = 0.8f;
    public bool canSkip = true;

    private string sceneKey;

    void Start()
    {
     
        string sceneName = SceneManager.GetActiveScene().name;
        sceneKey = "SubtitlePlayed_" + sceneName;

 
        if (PlayerPrefs.GetInt(sceneKey, 0) == 1)
        {
            HidePanel();
            return;
        }


        StartCoroutine(PlaySubtitle());
    }

    IEnumerator PlaySubtitle()
    {
        if (panelGroup != null)
            yield return StartCoroutine(FadeCanvas(panelGroup, 0, 1, fadeTime));

        foreach (string line in lines)
        {
            if (subtitleText == null) break;

            subtitleText.text = "";
            foreach (char c in line)
            {
                subtitleText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(lineDelay);
        }

        if (panelGroup != null)
            yield return StartCoroutine(FadeCanvas(panelGroup, 1, 0, fadeTime));

        HidePanel();

    
        PlayerPrefs.SetInt(sceneKey, 1);
        PlayerPrefs.Save();
    }

    void HidePanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.gameObject.SetActive(false);
        }
        if (subtitleText != null)
        {
            subtitleText.text = "";
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}
