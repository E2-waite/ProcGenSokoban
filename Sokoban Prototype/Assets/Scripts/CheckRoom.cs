using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class CheckRoom : MonoBehaviour
{
    public bool doors_opened = false;
    Room room = null;
    GenerateLevel level;
    private void Start()
    {
        level = GameObject.FindWithTag("Grid").GetComponent<GenerateLevel>();
    }

    public void StartChecking(Room _room)
    {
        room = _room;
        StartCoroutine(Check());
    }

    IEnumerator Check()
    {
        int buttons_pressed = 0;
        for (int y = 0; y < room.grid.GetLength(1); y++)
        {
            for (int x = 0; x < room.grid.GetLength(0); x++)
            {
                if (room.grid[x, y] == (int)Elements.floor + (int)Elements.button && room.object_grid[x,y].transform.childCount > 1)
                {
                    buttons_pressed++;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (buttons_pressed == room.num_boxes)
        {
            room.solved = true;
            OpenDoors();
        }
        else
        {
            room.solved = false;
            CloseDoors();
        }
        StartCoroutine(Check());
    }

    void OpenDoors()
    {
        if (!doors_opened)
        {
            doors_opened = true;
            for (int i = 0; i < room.exits.Count; i++)
            {
                room.exits[i].GetComponent<DoorAction>().Open();
                if (room.exit_dirs[i] == Direction.N)
                {
                    level.room_grid[room.pos.x, room.pos.y + 1].entrance.GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.E)
                {
                    level.room_grid[room.pos.x + 1, room.pos.y].entrance.GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.S)
                {
                    level.room_grid[room.pos.x, room.pos.y - 1].entrance.GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.W)
                {
                    level.room_grid[room.pos.x - 1, room.pos.y].entrance.GetComponent<DoorAction>().Open();
                }
            }

        }
    }

    void CloseDoors()
    {
        if (doors_opened)
        {
            doors_opened = false;
            for (int i = 0; i < room.exits.Count; i++)
            {
                room.exits[i].GetComponent<DoorAction>().Close();
                if (room.exit_dirs[i] == Direction.N)
                {
                    level.room_grid[room.pos.x, room.pos.y + 1].entrance.GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.E)
                {
                    level.room_grid[room.pos.x + 1, room.pos.y].entrance.GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.S)
                {
                    level.room_grid[room.pos.x, room.pos.y - 1].entrance.GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.W)
                {
                    level.room_grid[room.pos.x - 1, room.pos.y].entrance.GetComponent<DoorAction>().Close();
                }
            }
        }
    }
}
