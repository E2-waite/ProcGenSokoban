using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class CheckRoom : MonoBehaviour
{
    public bool doors_opened = false;
    bool started = false;
    Room room;
    GameControl game;
    private void Start()
    {
        game = GameObject.FindWithTag("Grid").GetComponent<GameControl>();
    }

    private void Update()
    {
        if (started)
        {
            float buttons_pressed = 0;
            for (int i = 0; i < room.buttons.Count; i++)
            {
                if (room.buttons[i].transform.parent.childCount == 2)
                {
                    buttons_pressed++;
                }
            }
            if (buttons_pressed == room.buttons.Count)
            {
                room.solved = true;
                OpenDoors();
            }
            else
            {
                room.solved = false;
                CloseDoors();
            }
        }
    }

    public void StartChecking(Room _room)
    {
        room = _room;
        started = true;
    }


    void OpenDoors()
    {
        if (room.last)
        {
            room.object_grid[room.exits[0].x, room.exits[0].y].GetComponent<DoorAction>().Open();
        }
        else
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
                        next_room.room_object.SetActive(true);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                    }
                    if (room.exit_dirs[i] == Direction.E)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x + 1, room.pos.y];
                        next_room.room_object.SetActive(true);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                    }
                    if (room.exit_dirs[i] == Direction.S)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y - 1];
                        next_room.room_object.SetActive(true);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                    }
                    if (room.exit_dirs[i] == Direction.W)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x - 1, room.pos.y];
                        next_room.room_object.SetActive(true);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Open();
                    }
                }
            }
        }
    }

    void CloseDoors()
    {
        if (room.last)
        {
            room.object_grid[room.exits[0].x, room.exits[0].y].GetComponent<DoorAction>().Close();
        }
        else
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
                        next_room.room_object.SetActive(false);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                    }
                    if (room.exit_dirs[i] == Direction.E)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x + 1, room.pos.y];
                        next_room.room_object.SetActive(false);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                    }
                    if (room.exit_dirs[i] == Direction.S)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x, room.pos.y - 1];
                        next_room.room_object.SetActive(false);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                    }
                    if (room.exit_dirs[i] == Direction.W)
                    {
                        Room next_room = game.this_level.room_grid[room.pos.x - 1, room.pos.y];
                        next_room.room_object.SetActive(false);
                        next_room.object_grid[next_room.entrance.x, next_room.entrance.y].GetComponent<DoorAction>().Close();
                    }
                }
            }
        }
    }
}
