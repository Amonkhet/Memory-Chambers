using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class SyncCollider : MonoBehaviour
{
    public Transform artRoot;
    public bool liveUpdate = true;
    public Vector3 padding = Vector3.zero;

    BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (artRoot == null)
        {
            var found = transform.parent != null ? transform.parent.Find("Art") : null;
            if (found != null) artRoot = found;
        }
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

        var renderers = artRoot.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds combinedBounds = renderers[0].bounds;
        foreach (var r in renderers)
            combinedBounds.Encapsulate(r.bounds);

        Vector3[] corners =
        {
            new(combinedBounds.min.x, combinedBounds.min.y, combinedBounds.min.z),
            new(combinedBounds.max.x, combinedBounds.min.y, combinedBounds.min.z),
            new(combinedBounds.min.x, combinedBounds.max.y, combinedBounds.min.z),
            new(combinedBounds.min.x, combinedBounds.min.y, combinedBounds.max.z),
            new(combinedBounds.max.x, combinedBounds.max.y, combinedBounds.min.z),
            new(combinedBounds.max.x, combinedBounds.min.y, combinedBounds.max.z),
            new(combinedBounds.min.x, combinedBounds.max.y, combinedBounds.max.z),
            new(combinedBounds.max.x, combinedBounds.max.y, combinedBounds.max.z),
        };

        Vector3 localMin = new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Vector3 localMax = new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        for (int i = 0; i < 8; i++)
        {
            var localCorner = transform.InverseTransformPoint(corners[i]);
            localMin = Vector3.Min(localMin, localCorner);
            localMax = Vector3.Max(localMax, localCorner);
        }

        boxCollider.center = (localMin + localMax) * 0.5f;
        boxCollider.size = (localMax - localMin) + padding;
    }
}