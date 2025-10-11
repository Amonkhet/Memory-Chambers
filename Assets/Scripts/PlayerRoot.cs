using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    // Make sure not destroy this
    public static PlayerRoot Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
