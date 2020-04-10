using System.Collections;
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
    public GameObject[,] Generate(Room room)
    {
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
                    room.parent_level.player = player;
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    boxes.Add(Instantiate(box_prefab, room.object_grid[x, y].transform));
                    boxes[boxes.Count - 1].transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                    room.parent_level.boxes.Add(boxes[boxes.Count - 1]);
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    buttons.Add(Instantiate(button_prefab, room.object_grid[x, y].transform));
                    buttons[buttons.Count - 1].transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                    room.buttons.Add(buttons[buttons.Count - 1]);
                }
                else if (room.grid[x, y] == (int)Elements.entrance)
                {
                    room.object_grid[x, y] = Instantiate(entrance_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                }
                else if (room.grid[x, y] == (int)Elements.exit)
                {
                    room.object_grid[x, y] = Instantiate(exit_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                }
                room.object_grid[x, y].transform.parent = room.room_object.transform;
                room.object_grid[x, y].name = x.ToString() + " " + y.ToString();
            }
        }
        HideWalls(room);

        room.room_object.GetComponent<CheckRoom>().StartChecking(room);
        if (!room.first)
        {
            room.room_object.SetActive(false);
        }
        return room.object_grid;
    }

    void HideWalls(Room room)
    {
        for (int y = 0; y < room.object_grid.GetLength(1); y++)
        {
            for (int x = 0; x < room.object_grid.GetLength(0); x++)
            {
                if (CheckDirs(room, new Pos { x = x, y = y }))
                {
                    Renderer rend = room.object_grid[x, y].GetComponent<Renderer>();
                    rend.enabled = false;
                }
            }
        }
    }

    bool CheckDirs(Room room, Pos pos)
    {
        int wall_dirs = 0;

        if (room.object_grid[pos.x, pos.y].CompareTag("Wall"))
        {
            if (pos.y + 1 >= room.object_grid.GetLength(1) || room.object_grid[pos.x, pos.y + 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x + 1 >= room.object_grid.GetLength(1) || pos.y + 1 >= room.object_grid.GetLength(1) || 
                room.object_grid[pos.x + 1, pos.y + 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x + 1 >= room.object_grid.GetLength(0) || room.object_grid[pos.x + 1, pos.y].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x + 1 >= room.object_grid.GetLength(0) || pos.y - 1 < 0 ||
                room.object_grid[pos.x + 1, pos.y - 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.y - 1 < 0 || room.object_grid[pos.x, pos.y - 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x - 1 < 0 || pos.y - 1 < 0 || 
                room.object_grid[pos.x - 1, pos.y - 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x - 1 < 0 || room.object_grid[pos.x - 1, pos.y].CompareTag("Wall"))
            {
                wall_dirs++;
            }
            if (pos.x - 1 < 0 || pos.y + 1 >= room.object_grid.GetLength(1) ||
                room.object_grid[pos.x - 1, pos.y + 1].CompareTag("Wall"))
            {
                wall_dirs++;
            }

            if (wall_dirs == 8)
            {
                return true;
            }
            else return false;
        }
        return false;
    }

    public void Delete(Room room)
    {
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
    }
}
