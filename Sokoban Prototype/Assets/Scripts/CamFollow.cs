using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    GameObject player = null;
    public float height = 10;

    public void SetPlayer(GameObject obj)
    {
        player = obj;
    }

    void Update()
    {
        if (player == null)
        {
            transform.position = new Vector3(5, height, 5);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, height, player.transform.position.z);
        }
    }
}
