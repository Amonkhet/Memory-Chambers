using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorToScene : MonoBehaviour
{
    // Attributes
    [Header("Switch to this scene")]
    public string sceneToSwitch;
    [Header("Activate range")]
    public float activateRange = 1.0f;
    [Header("Target player")]
    public Transform targetPlayer;

    private Camera camera;
    
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if left mouse click
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Door")))
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
        
    }
}
