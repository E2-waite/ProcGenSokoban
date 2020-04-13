using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAction : MonoBehaviour
{
    public bool open = false;
    public Type door_type;
    public enum Type
    {
        Door,
        Trapdoor
    }

    public void Close()
    {
        open = false;
        if (door_type == Type.Door)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            rend.enabled = true;
        }
    }

    public void Open()
    {
        open = true;
        if (door_type == Type.Door)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            rend.enabled = false;
        }
    }

    public bool IsOpen()
    {
        return open;
    }
}
