// Assets/Editor/SplitMeshByLoosePartsEditor.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SplitMeshByLoosePartsEditor
{
    [MenuItem("Tools/Mesh/Split Mesh By Loose Parts (Selection & Children)")]
    public static void SplitSelectedAndChildren()
    {
        var targets = Selection.gameObjects;
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("Select a GameObject in the Hierarchy first.");
            return;
        }

        int totalCreated = 0;
        foreach (var root in targets)
        {
            var mfs = root.GetComponentsInChildren<MeshFilter>(true);
            var mrs = root.GetComponentsInChildren<MeshRenderer>(true);

            if (mfs.Length == 0 || mrs.Length == 0)
            {
                Debug.LogWarning($"'{root.name}' has no MeshFilter/MeshRenderer children, skipped.");
                continue;
            }

            var mfSet = new HashSet<MeshFilter>(mfs);
            foreach (var mf in mfs)
            {
                var mr = mf.GetComponent<MeshRenderer>();
                if (!mr || !mf.sharedMesh) continue;

                totalCreated += SplitOne(mf.gameObject);
            }
        }

        Debug.Log($"Split complete. Created {totalCreated} new parts in total.");
    }

    static int SplitOne(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();
        var mr = go.GetComponent<MeshRenderer>();
        if (!mf || !mr || !mf.sharedMesh)
        {
            Debug.LogWarning($"'{go.name}' has no MeshFilter+MeshRenderer with a Mesh, skipped.");
            return 0;
        }

        var mesh = mf.sharedMesh;
        var verts = mesh.vertices;
        var tris  = mesh.triangles;

        // neighbors
        var adj = new List<int>[verts.Length];
        for (int i = 0; i < adj.Length; i++) adj[i] = new List<int>();
        for (int t = 0; t < tris.Length; t += 3)
        {
            int a = tris[t], b = tris[t + 1], c = tris[t + 2];
            adj[a].Add(b); adj[a].Add(c);
            adj[b].Add(a); adj[b].Add(c);
            adj[c].Add(a); adj[c].Add(b);
        }

        // DFS 
        int[] comp = new int[verts.Length];
        for (int i = 0; i < comp.Length; i++) comp[i] = -1;
        int compCount = 0;
        var stack = new Stack<int>();
        for (int i = 0; i < verts.Length; i++)
        {
            if (comp[i] != -1) continue;
            stack.Clear(); stack.Push(i); comp[i] = compCount;
            while (stack.Count > 0)
            {
                int v = stack.Pop();
                foreach (var nb in adj[v])
                    if (comp[nb] == -1) { comp[nb] = compCount; stack.Push(nb); }
            }
            compCount++;
        }

        // scan each triangle
        var trisPerComp = new List<int>[compCount];
        for (int i = 0; i < compCount; i++) trisPerComp[i] = new List<int>();
        for (int t = 0; t < tris.Length; t += 3)
        {
            int ca = comp[tris[t]], cb = comp[tris[t+1]], cc = comp[tris[t+2]];
            if (ca == cb && cb == cc)
            {
                trisPerComp[ca].Add(tris[t]);
                trisPerComp[ca].Add(tris[t+1]);
                trisPerComp[ca].Add(tris[t+2]);
            }
        }

        var normals  = mesh.normals;
        var tangents = mesh.tangents;
        var colors   = mesh.colors;
        var uv0      = mesh.uv;
        var uv1      = mesh.uv2;
        var uv2      = mesh.uv3;
        var uv3      = mesh.uv4;

        int created = 0;
        for (int ci = 0; ci < compCount; ci++)
        {
            var triList = trisPerComp[ci];
            if (triList.Count == 0) continue;

            var map = new Dictionary<int,int>();
            var newVerts = new List<Vector3>();
            var newNormals  = (normals  != null && normals.Length  == verts.Length)  ? new List<Vector3>() : null;
            var newTangents = (tangents != null && tangents.Length == verts.Length) ? new List<Vector4>() : null;
            var newColors   = (colors   != null && colors.Length   == verts.Length)  ? new List<Color>()   : null;
            var newUV0      = (uv0      != null && uv0.Length      == verts.Length)  ? new List<Vector2>() : null;
            var newUV1      = (uv1      != null && uv1.Length      == verts.Length)  ? new List<Vector2>() : null;
            var newUV2      = (uv2      != null && uv2.Length      == verts.Length)  ? new List<Vector2>() : null;
            var newUV3      = (uv3      != null && uv3.Length      == verts.Length)  ? new List<Vector2>() : null;
            var newTris     = new List<int>(triList.Count);

            foreach (var idx in triList)
            {
                if (!map.TryGetValue(idx, out int ni))
                {
                    ni = map[idx] = newVerts.Count;
                    newVerts.Add(verts[idx]);
                    if (newNormals  != null) newNormals.Add(normals[idx]);
                    if (newTangents != null) newTangents.Add(tangents[idx]);
                    if (newColors   != null) newColors.Add(colors[idx]);
                    if (newUV0      != null) newUV0.Add(uv0[idx]);
                    if (newUV1      != null) newUV1.Add(uv1[idx]);
                    if (newUV2      != null) newUV2.Add(uv2[idx]);
                    if (newUV3      != null) newUV3.Add(uv3[idx]);
                }
                newTris.Add(map[idx]);
            }

            var newMesh = new Mesh();
            newMesh.indexFormat = (newVerts.Count > 65535)
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16;
            newMesh.SetVertices(newVerts);
            newMesh.SetTriangles(newTris, 0, true);
            if (newNormals != null) newMesh.SetNormals(newNormals); else newMesh.RecalculateNormals();
            if (newTangents != null) newMesh.SetTangents(newTangents); else newMesh.RecalculateTangents();
            if (newColors != null)   newMesh.SetColors(newColors);
            if (newUV0 != null) newMesh.SetUVs(0, newUV0);
            if (newUV1 != null) newMesh.SetUVs(1, newUV1);
            if (newUV2 != null) newMesh.SetUVs(2, newUV2);
            if (newUV3 != null) newMesh.SetUVs(3, newUV3);
            newMesh.RecalculateBounds();

            var child = new GameObject(go.name + "_part_" + ci);
            child.transform.SetParent(go.transform, false); // keep local coordinate
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale    = Vector3.one;

            var cmf = child.AddComponent<MeshFilter>();
            var cmr = child.AddComponent<MeshRenderer>();
            cmf.sharedMesh = newMesh;
            cmr.sharedMaterials = mr.sharedMaterials;

            created++;
        }

        // cancel
        mr.enabled = false;
        return created;
    }
}
