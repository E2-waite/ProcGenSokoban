using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObjects : MonoBehaviour
{
    public GameObject floor_prefab, wall_prefab, player_prefab, box_prefab, button_prefab;
    private GameObject[,] object_grid;
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> boxes = new List<GameObject>();
    // Start is called before the first frame update
    public void Generate(int[,] grid, List<int[]> button_positions, List<int[]> box_positions)
    {
        object_grid = new GameObject[grid.GetLength(0), grid.GetLength(1)];
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == 1)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < button_positions.Count; i++)
        {
            buttons.Add(Instantiate(button_prefab, object_grid[button_positions[i][0], button_positions[i][1]].transform));
            buttons[buttons.Count - 1].transform.position = new Vector3(button_positions[i][0], 0.5f, button_positions[i][1]);
            buttons[buttons.Count - 1].name = "Button " + ((i + 1).ToString());
        }

        for (int i = 0; i < box_positions.Count; i++)
        {
            boxes.Add(Instantiate(box_prefab, object_grid[box_positions[i][0], box_positions[i][1]].transform));
            boxes[boxes.Count - 1].transform.position = new Vector3(box_positions[i][0], 0.5f, box_positions[i][1]);
            boxes[boxes.Count - 1].name = "Box " + ((i + 1).ToString());
        }
    }
}
