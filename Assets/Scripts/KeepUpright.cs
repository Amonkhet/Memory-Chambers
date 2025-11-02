using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        transform.eulerAngles = euler;
    }
}
