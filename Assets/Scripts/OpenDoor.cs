using System;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool hasOpened = false;

    void Update()
    {
        if (BookDropActivate.Dropped && !hasOpened)
        {
            hasOpened = true;
            gameObject.SetActive(true); 
        }
    }
}
