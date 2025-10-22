using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class PlayBuildingTimeline : MonoBehaviour
{
    [Header("Timeline manager")] 
    public WorkTimelineManager manager;

    [Header("TimelineRise or Fall")] 
    public bool playRiseTimeline = true;
    
    [Header("Play direction")]
    public bool playBackward = false;
    
    [Header("Activate key")]
    public KeyCode activateKey = KeyCode.Mouse1;
    public LayerMask clickMask = ~0;

    void Update()
    {
        if (Input.GetKeyDown(activateKey))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, clickMask))
            {
                if (hit.collider && (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform)))
                {
                    PlayTimeline();
                }
            }
        }
    }

    void PlayTimeline()
    {
        if (!manager)
        {
            return;
        }

        if (playRiseTimeline)
        {
            manager.PlayTimelineRise(playBackward);
        }
        else
        {
            manager.PlayTimelineFall(playBackward);
        }
        
    }
}
