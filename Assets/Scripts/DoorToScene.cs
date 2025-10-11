using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorToScene : MonoBehaviour
{
    // Attributes
    [Header("Switch to this scene")]
    public string targetScene;
    [Header("Activate range")]
    public float doorRange = 1.0f;
    [Header("Target player")]
    public Transform targetPlayer;
    //Layer = Door
    [Header("Layer")]
    [SerializeField] LayerMask doorLayer;
    private Camera camera;
    // Set player position in new scene
    [Header("Player position in new scene")]
    public Vector3 targetSceneSpawnPosition = Vector3.zero;
    public bool overrideRotation = false;
    public Vector3 targetSceneSpawnEuler = Vector3.zero;  // 可选朝向
    public string playerTagInNextScene = "Player"; 
    
    private static Vector3? s_pendingSpawnPos;
    private static Vector3? s_pendingSpawnEuler;
    private static string s_nextScenePlayerTag = "Player";
    
    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded_Static; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded_Static; }
    
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Get door position
        Vector3 GetDoorPosition()
        {
            return transform.position;
        }

        // Check if left mouse click
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, doorLayer))
        {
            return;
        }

        // Check if the ray hit the door collider
        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.transform != transform && hit.collider.GetComponentInParent<DoorToScene>() != this)
        {
            return;
        }

        //Check if player is near door range
        if (targetPlayer == null) return;
        float dist = Vector3.Distance(targetPlayer.position, GetDoorPosition());
        if (dist > doorRange)
        {
            return;
        }

        //Switch to different scene
        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"Loading scene: {targetScene}");
            // === ADD (just BEFORE LoadScene): 传递出生信息到下一场景 ===
            s_pendingSpawnPos    = targetSceneSpawnPosition;
            s_nextScenePlayerTag = string.IsNullOrEmpty(playerTagInNextScene) ? "Player" : playerTagInNextScene;
            s_pendingSpawnEuler  = overrideRotation ? (Vector3?)targetSceneSpawnEuler : null;
            SceneManager.LoadScene(targetScene);
        }
    }
    // === ADD: 新场景加载完成后放置玩家到指定点 ===
    private static void OnSceneLoaded_Static(Scene scene, LoadSceneMode mode)
    {
        if (!s_pendingSpawnPos.HasValue) return;  // 没有待处理出生点就直接返回

        GameObject playerGO = GameObject.FindGameObjectWithTag(s_nextScenePlayerTag);
        if (playerGO != null)
        {
            playerGO.transform.position = s_pendingSpawnPos.Value;
            if (s_pendingSpawnEuler.HasValue)
                playerGO.transform.rotation = Quaternion.Euler(s_pendingSpawnEuler.Value);
        }
        else
        {
            Debug.LogWarning($"[DoorToScene] 在场景 '{scene.name}' 中找不到 Tag='{s_nextScenePlayerTag}' 的玩家。");
        }

        // 一次性用完清空，避免影响后续切换
        s_pendingSpawnPos = null;
        s_pendingSpawnEuler = null;
        s_nextScenePlayerTag = "Player";
    }
}
