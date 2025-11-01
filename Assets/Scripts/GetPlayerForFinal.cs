using UnityEngine;
using UnityEngine.Playables;

public class GetPlayerForFinal : MonoBehaviour
{
    [SerializeField] private PlayableDirector director; 
    [SerializeField] private Animator lAnimator;      

    void Start()
    {
        var outputs = director.playableAsset.outputs;
        foreach (var o in outputs)
        {
            if (o.outputTargetType == typeof(Animator))
            {
                director.SetGenericBinding(o.sourceObject, lAnimator);
                // 如果有多条动画轨道，你也可以根据 o.streamName 判断只绑“LWalkTrack”
            }
        }
        
        lAnimator.applyRootMotion = true;

        director.time = 0;
        director.Play();
    }
}
