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
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("doorLayer")))
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
            SceneManager.LoadScene(targetScene);
        }
    }
}
