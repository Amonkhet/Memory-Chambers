using UnityEngine;
using System.IO;
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
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, videoFileName);
        #else
            videoPlayer.url = "file://" + Path.Combine(Application.streamingAssetsPath, videoFileName);
        #endif
    }

    public void PlayInstructionVideo()
    {
        if (videoPlayer != null && videoUI != null)
        {
           
            if (videoUI.activeSelf && videoPlayer.isPlaying)
            {
                
                CloseVideo();
            }
            else
            {
                
                videoUI.SetActive(true);
                videoPlayer.Play();
                videoPlayer.loopPointReached += OnVideoEnd;
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoUI.SetActive(false);
      
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    public void CloseVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoUI.SetActive(false);
           
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Video panel clicked!");
        CloseVideo();
    }
}