﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateObjects : MonoBehaviour
{
    public GameObject floor_prefab, wall_prefab, player_prefab, box_prefab, button_prefab, entrance_prefab, exit_prefab;
    GameObject player;
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> boxes = new List<GameObject>();

    // Start is called before the first frame update
    public void Generate(Room room)
    {
        // If there is a level already instantiated, delete it before instantiating the next
        if (room.object_grid != null)
        {
            for (int y = 0; y < room.object_grid.GetLength(1); y++)
            {
                for (int x = 0; x < room.object_grid.GetLength(0); x++)
                {
                    Destroy(room.object_grid[x, y]);
                }
            }
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }
            buttons.Clear();
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(boxes[i]);               
            }
            boxes.Clear();
            Destroy(player);
        }

        Pos entrance_pos = null, exit_pos = null;
        room.object_grid = new GameObject[room.grid.GetLength(0), room.grid.GetLength(1)];
        for (int y = 0; y < room.grid.GetLength(1); y++)
        {
            for (int x = 0; x < room.grid.GetLength(0); x++)
            {
                if (room.grid[x, y] == (int)Elements.floor)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                }
                else if (room.grid[x, y] == (int)Elements.wall)
                {
                    room.object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.player)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    player = Instantiate(player_prefab, room.object_grid[x, y].transform);
                    player.transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    boxes.Add(Instantiate(box_prefab, room.object_grid[x, y].transform));
                    boxes[boxes.Count - 1].transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    buttons.Add(Instantiate(button_prefab, room.object_grid[x, y].transform));
                    buttons[buttons.Count - 1].transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                }
                else if (room.grid[x, y] == (int)Elements.entrance)
                {
                    room.object_grid[x, y] = Instantiate(entrance_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    entrance_pos = new Pos { x = x, y = y };
                }
                else if (room.grid[x, y] == (int)Elements.exit)
                {
                    room.object_grid[x, y] = Instantiate(exit_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    exit_pos = new Pos { x = x, y = y };
                }
                room.object_grid[x, y].transform.parent = room.room_object.transform;
                room.object_grid[x, y].name = x.ToString() + " " + y.ToString();
            }
        }

        room.generated = true;
        GetComponent<CheckRoom>().StartChecking(room);
        StartCoroutine(CheckPath(room, entrance_pos, exit_pos));
    }

    IEnumerator CheckPath(Room room, Pos entrance_pos, Pos exit_pos)
    {
        FindPath path = new FindPath(room.grid, entrance_pos, exit_pos);
        bool checking = true;
        while (checking)
        {
            if (path.final_path.Count > 0)
            {
                Debug.Log("PATH EXISTS");
                checking = false;
            }
            if (path.path_failed)
            {
                Debug.Log("PATH FAILED");
                checking = false;
            }
            yield return null;
        }
    }
}
