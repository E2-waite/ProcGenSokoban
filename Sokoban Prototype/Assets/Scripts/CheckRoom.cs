using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class CheckRoom : MonoBehaviour
{
    public void StartChecking(Room room)
    {
        Debug.Log("CHECKING ROOM");
        StartCoroutine(Check(room));
    }

    IEnumerator Check(Room room)
    {
        int buttons_pressed = 0;
        for (int y = 0; y < room.grid.GetLength(1); y++)
        {
            for (int x = 0; x < room.grid.GetLength(0); x++)
            {
                if (room.grid[x, y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
                {
                    buttons_pressed++;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (buttons_pressed == room.num_boxes)
        {
            room.solved = true;
        }
        else
        {
            room.solved = false;
        }
        StartCoroutine(Check(room));
    }
}
