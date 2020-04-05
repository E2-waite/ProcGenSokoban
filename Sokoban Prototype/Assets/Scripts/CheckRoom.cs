using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class CheckRoom : MonoBehaviour
{
    public bool doors_opened = false;
    Room room = null;
    GameControl game;
    private void Start()
    {
        game = GameObject.FindWithTag("Grid").GetComponent<GameControl>();
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

        yield return new WaitForEndOfFrame();
        if (buttons_pressed == room.num_boxes)
        {
            room.solved = true;
            if (room.last)
            {
                // Instantiate next level
                game.level_won = true;
            }
            else
            {
                OpenDoors();
            }
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
            Debug.Log("OPENING DOORS");
            doors_opened = true;
            for (int i = 0; i < room.exits.Count; i++)
            {
                room.object_grid[room.exits[i].x, room.exits[i].y].GetComponent<DoorAction>().Open();
                if (room.exit_dirs[i] == Direction.N)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y + 1];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.E)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x + 1, room.pos.y];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.S)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y - 1];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                }
                if (room.exit_dirs[i] == Direction.W)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x - 1, room.pos.y];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
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
                room.object_grid[room.exits[i].x, room.exits[i].y].GetComponent<DoorAction>().Close();
                if (room.exit_dirs[i] == Direction.N)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y + 1];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.E)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x + 1, room.pos.y];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.S)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y - 1];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                }
                if (room.exit_dirs[i] == Direction.W)
                {
                    Room next_room = game.this_level.room_grid[room.pos.x - 1, room.pos.y];
                    next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                }
            }
        }
    }
}
