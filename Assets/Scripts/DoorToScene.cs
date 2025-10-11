using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToScene : MonoBehaviour
{
    [Header("Switch to this scene")]
    public string targetScene;
    [Header("Activate range")]
    public float doorRange = 1.0f;
    [Header("Target player")]
    public Transform targetPlayer;
    [Header("Layer")]
    [SerializeField] LayerMask doorLayer;
    private Camera camera;

    [Header("Player position in new scene")]
    public Vector3 targetSceneSpawnPosition = Vector3.zero;
    public bool overrideRotation = false;
    public Vector3 targetSceneSpawnEuler = Vector3.zero;
    public string playerTagInNextScene = "Player";

    private static Vector3? s_pendingSpawnPos;
    private static Vector3? s_pendingSpawnEuler;
    private static string s_nextScenePlayerTag = "Player";

    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded_Static; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded_Static; }

    void Start()
    {
        camera = Camera.main;
        Debug.Log($"[{name}] DoorToScene initialized. targetScene = {targetScene}, doorRange = {doorRange}");
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Debug.Log($"[{name}] ① 鼠标点击检测到");

        // 1️⃣ 发射射线
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, doorLayer))
        {
            Debug.Log($"[{name}] ② 射线未命中任何 Collider (layer mask = {doorLayer.value})");
            return;
        }
        Debug.Log($"[{name}] ② 射线命中对象: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

        // 2️⃣ 是否命中本门
        if (hit.collider == null)
        {
            Debug.Log($"[{name}] ③ hit.collider 为空");
            return;
        }

        if (hit.collider.transform != transform && hit.collider.GetComponentInParent<DoorToScene>() != this)
        {
            Debug.Log($"[{name}] ④ 命中对象不是当前门({hit.collider.transform.name})，已返回");
            return;
        }

        // 3️⃣ 玩家距离检测
        if (targetPlayer == null)
        {
            Debug.LogError($"[{name}] ⑤ targetPlayer 未设置！");
            return;
        }

        float dist = Vector3.Distance(targetPlayer.position, transform.position);
        Debug.Log($"[{name}] ⑥ 玩家距离门中心: {dist:F2}");

        if (dist > doorRange)
        {
            Debug.Log($"[{name}] ⑦ 玩家太远({dist:F2}>{doorRange})，不切场景");
            return;
        }

        // 4️⃣ 准备切场景
        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"[{name}] ⑧ 准备切换到场景: {targetScene}");
            Debug.Log($"[{name}] 传递出生点: {targetSceneSpawnPosition}, overrideRotation={overrideRotation}, euler={targetSceneSpawnEuler}");

            s_pendingSpawnPos    = targetSceneSpawnPosition;
            s_nextScenePlayerTag = string.IsNullOrEmpty(playerTagInNextScene) ? "Player" : playerTagInNextScene;
            s_pendingSpawnEuler  = overrideRotation ? (Vector3?)targetSceneSpawnEuler : null;

            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning($"[{name}] ⑨ targetScene 未填写，无法切换");
        }
    }

    private static void OnSceneLoaded_Static(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[DoorToScene] ⑩ OnSceneLoaded_Static called for scene '{scene.name}'");

        if (!s_pendingSpawnPos.HasValue)
        {
            Debug.Log("[DoorToScene] ⑪ 没有待处理的出生点数据，直接返回");
            return;
        }

        GameObject playerGO = GameObject.FindGameObjectWithTag(s_nextScenePlayerTag);
        if (playerGO != null)
        {
            Debug.Log($"[DoorToScene] ⑫ 找到玩家 '{playerGO.name}'，设置位置={s_pendingSpawnPos.Value}");
            playerGO.transform.position = s_pendingSpawnPos.Value;
            if (s_pendingSpawnEuler.HasValue)
            {
                Debug.Log($"[DoorToScene] ⑬ 设置旋转={s_pendingSpawnEuler.Value}");
                playerGO.transform.rotation = Quaternion.Euler(s_pendingSpawnEuler.Value);
            }
        }
        else
        {
            Debug.LogWarning($"[DoorToScene] ⑭ 在场景 '{scene.name}' 中找不到 Tag='{s_nextScenePlayerTag}' 的玩家。");
        }

        Debug.Log("[DoorToScene] ⑮ 清空缓存数据");
        s_pendingSpawnPos = null;
        s_pendingSpawnEuler = null;
        s_nextScenePlayerTag = "Player";
    }
}