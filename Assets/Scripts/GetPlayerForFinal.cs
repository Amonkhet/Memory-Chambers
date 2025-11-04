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
            }
        }
        
        lAnimator.applyRootMotion = true;

        director.time = 0;
        director.Play();
    }
}
