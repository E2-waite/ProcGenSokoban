using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateObjects : MonoBehaviour
{
    public GameObject floor_prefab, wall_prefab, player_prefab, box_prefab, button_prefab, entrance_prefab, exit_prefab;
    public GameObject[,] object_grid;
    GameObject player;
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> boxes = new List<GameObject>();
    bool generated = false;
    // Start is called before the first frame update
    public void Generate(int[,] grid)
    {
        // If there is a level already instantiated, delete it before instantiating the next
        if (generated)
        {
            for (int y = 0; y < object_grid.GetLength(1); y++)
            {
                for (int x = 0; x < object_grid.GetLength(0); x++)
                {
                    Destroy(object_grid[x, y]);
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
                else if (grid[x, y] == (int)Elements.entrance)
                {
                    object_grid[x, y] = Instantiate(entrance_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    entrance_pos = new Pos { x = x, y = y };
                }
                else if (grid[x, y] == (int)Elements.exit)
                {
                    object_grid[x, y] = Instantiate(exit_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    exit_pos = new Pos { x = x, y = y };
                }
                else
                {
                    Debug.Log("WAS " + grid[x,y].ToString());
                }
            }
        }

        StartCoroutine(CheckPath(grid, entrance_pos, exit_pos));

        generated = true;
        GetComponent<GameControl>().StartGame(grid, object_grid, boxes.Count);
    }

    IEnumerator CheckPath(int[,] grid, Pos entrance_pos, Pos exit_pos)
    {
        FindPath path = new FindPath(grid, entrance_pos, exit_pos);
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
