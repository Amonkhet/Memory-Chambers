using UnityEngine;
using UnityEngine.Playables;

public class PlayBuildingTimeline : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector timelineRise;
    public PlayableDirector timelineFall;

    public bool risePlayEnd = false;
    public bool fallPlayEnd = false;
    private bool isPlaying;
    // Control whether to play time forward or backward
    
}
