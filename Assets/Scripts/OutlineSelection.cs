using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    private Outline currentOutline;
    private Outline selectedOutline;
    private RaycastHit hit;

    [Header("Outline Settings")]
    public Color hoverColor = Color.yellow;
    public float outlineWidth = 7f;

    void Update()
    {
        // Disable previous highlight if needed
        if (currentOutline) currentOutline.enabled = false;
        currentOutline = null;

        // Mouse hover detection
        if (!EventSystem.current.IsPointerOverGameObject() &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                // Get or add Outline component
                currentOutline = hit.transform.GetComponent<Outline>();
                if (currentOutline == null)
                    currentOutline = hit.transform.gameObject.AddComponent<Outline>();

                // Apply settings
                currentOutline.OutlineColor = hoverColor;
                currentOutline.OutlineWidth = outlineWidth;
                currentOutline.enabled = true;

                // 🔹 NEW PART: Force outline to render after normal geometry
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend != null)
                {
                    foreach (var mat in rend.materials)
                    {
                        // Default opaque queue = 2000; transparent = 3000
                        // We set it a bit later so it draws over the player
                        mat.renderQueue = 3100;
                    }
                }
            }
        }

        // Mouse click selection
        if (Input.GetMouseButtonDown(0))
        {
            if (currentOutline)
            {
                if (selectedOutline) selectedOutline.enabled = false;
                selectedOutline = currentOutline;
                currentOutline = null;
            }
            else if (selectedOutline)
            {
                selectedOutline.enabled = false;
                selectedOutline = null;
            }
        }
    }
}
