using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSurrounding : MonoBehaviour
{
    public void CheckAdjascentFloor (GameObject[,] object_grid, GameObject grid, bool boxes_placed)
    {
        StartCoroutine(CheckFloor(object_grid, grid, boxes_placed));
    }

    IEnumerator CheckFloor(GameObject[,] object_grid, GameObject grid, bool boxes_placed)
    {
        GenerateGrid grid_script = grid.GetComponent<GenerateGrid>();
        int x = (int)transform.position.x, y = (int)transform.position.z;
        for (int i = 0; i < 4; i++)
        {
            int check_x = 0, check_y = 0;
            if (i == 0) check_y = y + 1;
            if (i == 1) check_x = x + 1;
            if (i == 2) check_y = y - 1;
            if (i == 3) check_x = x - 1;
            Debug.Log("X" + check_x.ToString() + " Y" + check_y.ToString());
            if (check_x >= 0 && check_x < object_grid.GetLength(0) && check_y >= 0 && check_y < object_grid.GetLength(1))
            {
                Debug.Log("1");
                if (grid_script.floor_tiles.Contains(object_grid[check_x, check_y]) && !grid_script.checked_floors.Contains(object_grid[check_x, check_y]))
                {
                    Debug.Log("2");
                    if (object_grid[check_x, check_y].transform.childCount == 0 || !object_grid[check_x, check_y].transform.GetChild(0).CompareTag("Box"))
                    {
                        Debug.Log("3");
                        grid_script.checked_floors.Add(object_grid[check_x, check_y]);
                        yield return new WaitForSeconds(0.01f);
                        CheckSurrounding next_check = object_grid[check_x, check_y].GetComponent<CheckSurrounding>();
                        next_check.CheckAdjascentFloor(object_grid, grid, boxes_placed);
                    }
                }
            }
        }
    }
}
