using System.Collections;
using UnityEngine;
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
        "The desk, the bed, the shelves — everything looks so familiar, yet so distant.",
        "Why am I so small? Have I shrunk, or has this world grown around me?",
        "I feel trapped inside my own design — a space that once defined me.",
        "I need to get out... I can’t stay here any longer."
    };

    [Header("Timing Settings")]
    public float typingSpeed = 0.06f;
    public float lineDelay = 1.6f;
    public float fadeTime = 0.8f;
    public bool canSkip = true;

    private bool isPlaying = false;

    void Start()
    {
   
        if (PlayerPrefs.GetInt("RoomLife_Intro_Played", 0) == 1)
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
            return;
        }

        // only play in first time 
        StartCoroutine(PlaySubtitle());
    }

    IEnumerator PlaySubtitle()
    {
        isPlaying = true;

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

                if (canSkip && Input.anyKeyDown)
                {
                    subtitleText.text = line;
                    break;
                }
            }
            yield return new WaitForSeconds(lineDelay);
        }

        if (panelGroup != null)
            yield return StartCoroutine(FadeCanvas(panelGroup, 1, 0, fadeTime));

        if (panelGroup != null)
            panelGroup.gameObject.SetActive(false);

        isPlaying = false;

   
        PlayerPrefs.SetInt("RoomLife_Intro_Played", 1);
        PlayerPrefs.Save();
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;

        float t = 0;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }


    public void ResetSubtitlePlayedFlag()
    {
        PlayerPrefs.DeleteKey("RoomLife_Intro_Played");
    }
}
