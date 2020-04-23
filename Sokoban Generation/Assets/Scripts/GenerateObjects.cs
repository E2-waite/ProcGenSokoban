using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateObjects : MonoBehaviour
{
    public GameObject floor_prefab, wall_prefab, player_prefab, box_prefab, button_prefab, entrance_prefab, exit_prefab, trapdoor_prefab, dead_prefab;
    GameObject player;
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> boxes = new List<GameObject>();

    // Start is called before the first frame update
    public GameObject[,] Generate(Room room, Level level, bool dead_cross, bool hide_rooms)
    {
        room.buttons = new List<GameObject>();
        room.object_grid = new GameObject[room.grid.GetLength(0), room.grid.GetLength(1)];
        for (int y = 0; y < room.grid.GetLength(1); y++)
        {
            for (int x = 0; x < room.grid.GetLength(0); x++)
            {
                if (room.grid[x, y] == (int)Elements.floor)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x,y] == (int)Elements.dead)
                {
                    if (dead_cross)
                    {
                        room.object_grid[x, y] = Instantiate(dead_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    }
                    else
                    {
                        room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    }
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.wall)
                {
                    room.object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.player)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    player = Instantiate(player_prefab, room.object_grid[x, y].transform);
                    player.transform.position = new Vector3(x + room.offset_x, 6, y + room.offset_y);
                    room.parent_level.player = player;
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    boxes.Add(Instantiate(box_prefab, room.object_grid[x, y].transform));
                    boxes[boxes.Count - 1].transform.position = new Vector3(x + room.offset_x, 1, y + room.offset_y);
                    level.boxes.Add(boxes[boxes.Count - 1]);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    room.object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    buttons.Add(Instantiate(button_prefab, room.object_grid[x, y].transform));
                    buttons[buttons.Count - 1].transform.position = new Vector3(x + room.offset_x, 0.5f, y + room.offset_y);
                    room.buttons.Add(buttons[buttons.Count - 1]);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.entrance)
                {
                    room.object_grid[x, y] = Instantiate(entrance_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x, y] == (int)Elements.exit)
                {
                    room.object_grid[x, y] = Instantiate(exit_prefab, new Vector3(x + room.offset_x, 1, y + room.offset_y), Quaternion.identity);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x,y ] == (int)Elements.trapdoor)
                {
                    room.object_grid[x, y] = Instantiate(trapdoor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                else if (room.grid[x,y] == (int)Elements.trapdoor + (int)Elements.player)
                {
                    room.object_grid[x, y] = Instantiate(trapdoor_prefab, new Vector3(x + room.offset_x, 0, y + room.offset_y), Quaternion.identity);
                    player = Instantiate(player_prefab, new Vector3(x + room.offset_x, 6, y + room.offset_y),Quaternion.identity);
                    player.transform.parent = room.object_grid[x, y].transform;
                    room.parent_level.player = player;
                    level.object_grid[x + room.offset_x, y + room.offset_y] = room.object_grid[x, y];
                }
                room.object_grid[x, y].transform.parent = room.room_object.transform;
                room.object_grid[x, y].name = x.ToString() + " " + y.ToString();
            }
        }

        HideWalls(room);

        if (player)
        {
            StartCoroutine(player.GetComponent<Player>().FallIn());
        }

        room.room_object.GetComponent<CheckRoom>().StartChecking(room);
        if (!room.first && hide_rooms)
        {
            room.room_object.SetActive(false);
        }
        room.instantiated = true;
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
            if (pos.x + 1 >= room.object_grid.GetLength(0) || pos.y + 1 >= room.object_grid.GetLength(1) || 
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
}
