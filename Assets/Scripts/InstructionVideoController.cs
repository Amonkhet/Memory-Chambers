using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class InstructionVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoUI; // The Raw Image panel
    public string videoFileName = "instruction.mp4";
    void Start()
    {
#if UNITY_WEBGL
        // Load video from StreamingAssets path
        string videoPath = System.IO.Path.Combine(Application.VideosPath, videoFileName);
        videoPlayer.url = videoPath;
#endif
    }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Video panel clicked!");
        CloseVideo();
    }
}

