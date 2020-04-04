using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAction : MonoBehaviour
{
    bool open = false;

    public void Close()
    {
        open = false;
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    public void Open()
    {
        open = true;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public bool IsOpen()
    {
        return open;
    }
}
