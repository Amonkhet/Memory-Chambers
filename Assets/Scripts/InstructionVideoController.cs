using UnityEngine;
using UnityEngine.Video;

public class InstructionVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoUI; // The Raw Image panel

    public void PlayInstructionVideo()
    {
        if (videoPlayer != null && videoUI != null)
        {
            videoUI.SetActive(true);
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoUI.SetActive(false);
    }

    public void CloseVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoUI.SetActive(false);
        }
    }
}

