using UnityEngine;
using UnityEngine.Playables;

public class BuildingTimelineManager : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector timelineRise;
    public PlayableDirector timelineFall;

    public bool risePlayEnd = false;
    public bool fallPlayEnd = false;
    private bool isPlaying;

    // Play timeline
    void PlayTimeline(PlayableDirector playableDirector, bool playBackward)
    {
        const double EPS = 0.001;
        if (playBackward)
        {
            playableDirector.time = playableDirector.duration - EPS;
            playableDirector.Evaluate();
            var root = playableDirector.playableGraph.GetRootPlayable(0);
            if (root.IsValid())
            {
                root.SetSpeed(-1.0);
            }
        }
        else
        {
            playableDirector.time = EPS;
            playableDirector.Evaluate();
            var root = playableDirector.playableGraph.GetRootPlayable(0);
            if (root.IsValid())
            {
                root.SetSpeed(1.0);
            }
        }
        isPlaying = true;
        playableDirector.Play();
    }
    // Stop play other timeline while playing this timeline
    void StopOtherTimeline(PlayableDirector director)
    {
        if (director && director.state == PlayState.Playing)
        {
            director.Stop();
        }
    }
    // Play timeline rise
    public void PlayTimelineRise(bool playBackward)
    {
        if (isPlaying || !timelineRise)
        {
            return;
        }

        StopOtherTimeline(timelineFall);
        PlayTimeline(timelineRise, playBackward);
    }
    // Play timeline fall
    public void PlayTimelineFall(bool playBackward)
    {
        if (isPlaying || !timelineFall)
        {
            return;
        }
        StopOtherTimeline(timelineFall);
        PlayTimeline(timelineFall, playBackward);
    }
    
    
}
