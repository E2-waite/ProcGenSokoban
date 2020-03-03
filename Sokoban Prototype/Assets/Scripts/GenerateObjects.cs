using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateObjects : MonoBehaviour
{
    public GameObject floor_prefab, wall_prefab, player_prefab, box_prefab, button_prefab;
    public GameObject[,] object_grid;
    GameObject player;
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> boxes = new List<GameObject>();
    // Start is called before the first frame update
    public void Generate(int[,] grid)
    {
        object_grid = new GameObject[grid.GetLength(0), grid.GetLength(1)];
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == (int)Elements.floor)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                }
                else if (grid[x, y] == (int)Elements.wall)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                }
                else if (grid[x, y] == (int)Elements.floor + (int)Elements.player)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    player = Instantiate(player_prefab, object_grid[x, y].transform);
                    player.transform.position = new Vector3(x, 0.5f, y);
                }
                else if (grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    boxes.Add(Instantiate(box_prefab, object_grid[x, y].transform));
                    boxes[boxes.Count - 1].transform.position = new Vector3(x, 0.5f, y);
                }
                else if (grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    buttons.Add(Instantiate(button_prefab, object_grid[x, y].transform));
                    buttons[buttons.Count - 1].transform.position = new Vector3(x, 0.5f, y);
                }
            }
        }
    }
}
