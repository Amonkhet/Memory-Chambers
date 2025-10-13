using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class SyncCollider : MonoBehaviour
{
    public Transform artRoot;
    public bool liveUpdate = true;
    public Vector3 padding = Vector3.zero;

    BoxCollider boxCollider;

    void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (artRoot == null && transform.parent != null)
        {
            var t = transform.parent.Find("Art");
            if (t) artRoot = t;
        }
        FitCollider();
    }

    void OnValidate()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        FitCollider();
    }

    void Update()
    {
        if (!Application.isPlaying && liveUpdate)
            FitCollider();
    }

    [ContextMenu("Fit Collider Now")]
    public void FitCollider()
    {
        if (boxCollider == null || artRoot == null) return;

        var renderers = artRoot.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;

        var toLocal = transform.worldToLocalMatrix;

        Vector3 min = new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Vector3 max = new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        foreach (var r in renderers)
        {
            var lb = r.localBounds;               // OBB in renderer local space
            var c = lb.center;
            var e = lb.extents;

            // 8 corners in renderer local space
            Vector3[] corners =
            {
                c + new Vector3( e.x,  e.y,  e.z),
                c + new Vector3( e.x,  e.y, -e.z),
                c + new Vector3( e.x, -e.y,  e.z),
                c + new Vector3( e.x, -e.y, -e.z),
                c + new Vector3(-e.x,  e.y,  e.z),
                c + new Vector3(-e.x,  e.y, -e.z),
                c + new Vector3(-e.x, -e.y,  e.z),
                c + new Vector3(-e.x, -e.y, -e.z),
            };

            var l2w = r.localToWorldMatrix;
            for (int i = 0; i < 8; i++)
            {
                var worldP = l2w.MultiplyPoint3x4(corners[i]);   // to world
                var p = toLocal.MultiplyPoint3x4(worldP);        // to collider local
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        }

        boxCollider.center = (min + max) * 0.5f;
        boxCollider.size   = (max - min) + padding;
    }
}