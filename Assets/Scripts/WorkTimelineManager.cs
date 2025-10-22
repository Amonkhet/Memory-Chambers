using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class WorkTimelineManager : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector timelineRise;
    public PlayableDirector timelineFall;
    private bool isPlaying;

    void Awake()
    {
        if (timelineRise)  timelineRise.stopped  += OnPlayStopped;
        if (timelineFall)  timelineFall.stopped  += OnPlayStopped;
    }

    // When timeline stop playing
    void OnPlayStopped(PlayableDirector d)
    {
        isPlaying = false;
    }

    // Play timeline
    void PlayTimeline(PlayableDirector playableDirector, bool playBackward)
    {
        if (!playableDirector)
        {
            return;
        }

        const double EPS = 0.001;
        // Stop this timeline if it is already playing
        if (playableDirector.state == PlayState.Playing)
        {
            playableDirector.Stop();
        }

        if (playBackward)
        {
            // Play backward from the end
            playableDirector.time = Mathf.Clamp((float)(playableDirector.duration - EPS), (float)EPS, (float)(playableDirector.duration - EPS));
            playableDirector.Evaluate();
            var root = playableDirector.playableGraph.GetRootPlayable(0);
            if (root.IsValid())
            {
                root.SetSpeed(-1.0);
            }
        }
        else
        {
            // Play forward from the start
            playableDirector.time = (float)EPS;
            playableDirector.Evaluate();
            var root = playableDirector.playableGraph.GetRootPlayable(0);
            if (root.IsValid())
            {
                root.SetSpeed(1.0);
            }
        }

        isPlaying = true;
        playableDirector.Play();
        StartCoroutine(AutoUnlockAfter(playableDirector));
    }

    // Prevent lock 
    IEnumerator AutoUnlockAfter(PlayableDirector playableDirector)
    {
        var root = playableDirector.playableGraph.GetRootPlayable(0);
        double speed = root.IsValid() ? root.GetSpeed() : 1.0;
        double remaining = (speed >= 0)
            ? Mathf.Max(0.01f, (float)(playableDirector.duration - playableDirector.time))
            : Mathf.Max(0.01f, (float)(playableDirector.time));

        yield return new WaitForSeconds((float)remaining + 0.2f);

        if (isPlaying)
        {
            try { playableDirector.Stop(); } catch {}
            isPlaying = false;
        }
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

        StopOtherTimeline(timelineRise);
        PlayTimeline(timelineFall, playBackward);
    }
}