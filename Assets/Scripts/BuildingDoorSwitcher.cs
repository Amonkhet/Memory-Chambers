using UnityEngine;

public class BuildingDoorSwitcher : MonoBehaviour
{
    // Attribute
    [Header("Building Doors")]
    [SerializeField] private GameObject buildingDoorA;
    [SerializeField] private GameObject buildingDoorB;
    [SerializeField] private GameObject buildingDoorC;
    [SerializeField] private GameObject buildingDoorD;
    
    [Header("Default Door state")]
    public bool isDoorAActive = true;
    public bool isDoorBActive = true;
    public bool isDoorCActive = false;
    public bool isDoorDActive = false;
    
    [Header("Default Door collider state")]
    public bool isDoorAColliderActive = true;
    public bool isDoorBColliderActive = true;
    public bool isDoorCColliderActive = false;
    public bool isDoorDColliderActive = false;
    // Get door collider

    void Start()
    {
        ChangeDoorState();

    }
    // Switch door
    public void SwitchDoor(string doorID)
    {
        switch (doorID)
        {
            case "A":
                isDoorAActive = true;
                isDoorBActive = false;
                isDoorCActive = true;
                isDoorDActive = false;
                
                isDoorAColliderActive = false;
                isDoorBColliderActive = false;
                isDoorCColliderActive = true;
                isDoorDColliderActive = false;
                break;
            case "B":
                isDoorAActive = false;
                isDoorBActive = true;
                isDoorCActive = false;
                isDoorDActive = true;
                
                isDoorAColliderActive = false;
                isDoorBColliderActive = false;
                isDoorCColliderActive = false;
                isDoorDColliderActive = true;
                break;
            case "C":
                isDoorAActive = true;
                isDoorBActive = true;
                isDoorCActive = false;
                isDoorDActive = false;
                
                isDoorAColliderActive = true;
                isDoorBColliderActive = true;
                isDoorCColliderActive = false;
                isDoorDColliderActive = false;
                break;
            case "D":
                isDoorAActive = true;
                isDoorBActive = true;
                isDoorCActive = false;
                isDoorDActive = false;
                
                isDoorAColliderActive = true;
                isDoorBColliderActive = true;
                isDoorCColliderActive = false;
                isDoorDColliderActive = false;
                break;
        }
        ChangeDoorState();
    }
    // Change door state
    
    void ChangeDoorState()
    {
        if (buildingDoorA)
        {
            buildingDoorA.SetActive(isDoorAActive);
        }

        if (buildingDoorB)
        {
            buildingDoorB.SetActive(isDoorBActive);
        }

        if (buildingDoorC)
        {
            buildingDoorC.SetActive(isDoorCActive);
        }

        if (buildingDoorD)
        {
            buildingDoorD.SetActive(isDoorDActive);
        }
        EnableCollider(buildingDoorA, isDoorAColliderActive);
        EnableCollider(buildingDoorB, isDoorBColliderActive);
        EnableCollider(buildingDoorC, isDoorCColliderActive);
        EnableCollider(buildingDoorD, isDoorDColliderActive);
    }

    void EnableCollider(GameObject door, bool enable)
    {
        if (!door) return;
        // Get collider on child
        Collider[] colliders = door.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders)
        {
            col.enabled = enable;
        }
    }
}
