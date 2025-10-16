using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class BookColorizer : MonoBehaviour
{
    public Color[] palette = new Color[] {
        new Color(0.90f,0.35f,0.30f),
        new Color(0.95f,0.64f,0.26f),
        new Color(0.97f,0.80f,0.31f),
        new Color(0.43f,0.72f,0.45f),
        new Color(0.34f,0.61f,0.76f),
        new Color(0.51f,0.52f,0.82f),
        new Color(0.72f,0.45f,0.73f),
        new Color(0.62f,0.55f,0.51f),
        new Color(0.86f,0.67f,0.53f)
    };

    public string baseColorProperty = "_BaseColor";
    public string useAlbedoProperty = "_UseAlbedo";
    public bool forceDisableAlbedo = true;

    MaterialPropertyBlock _mpb;

    void OnEnable()  { Apply(); }
    void OnValidate(){ Apply(); }

    public void Apply()
    {
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        var renderers = GetComponentsInChildren<MeshRenderer>(true);
        var list = new List<MeshRenderer>(renderers);

        list.Sort((a,b)=>a.bounds.center.x.CompareTo(b.bounds.center.x));

        int last = -1;
        for (int i = 0; i < list.Count; i++)
        {
            var r = list[i];
            int idx = i % palette.Length;
            if (idx == last) idx = (idx + 1) % palette.Length;
            last = idx;

            r.GetPropertyBlock(_mpb);
            if (forceDisableAlbedo && !string.IsNullOrEmpty(useAlbedoProperty))
                _mpb.SetFloat(useAlbedoProperty, 0f);
            _mpb.SetColor(baseColorProperty, palette[idx]);
            r.SetPropertyBlock(_mpb);
        }
    }
}
