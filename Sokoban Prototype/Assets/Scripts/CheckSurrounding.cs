using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSurrounding : MonoBehaviour
{
    public bool CheckAdjascentWalls(List<GameObject> tiles)
    {
        int num_walls = 0;
        foreach (GameObject obj in tiles)
        {
            if (obj.tag == "Wall" && Vector3.Distance(transform.position, obj.transform.position) < 1.5f)
            {
                num_walls++;                
            }
        }

        if (num_walls > 2) return true;
        else return false;
    }
    public void CheckAdjascentFloor (GameObject[,] object_grid, GameObject grid, bool boxes_placed)
    {
        StartCoroutine(CheckFloor(object_grid, grid, boxes_placed));
    }

    IEnumerator CheckFloor(GameObject[,] object_grid, GameObject grid, bool boxes_placed)
    {
        GenerateGrid grid_script = grid.GetComponent<GenerateGrid>();
        foreach (GameObject obj in object_grid)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < 1.3f && obj.CompareTag("Floor"))
            {
                grid_script.AddToChecked(obj);               
                yield return new WaitForSeconds(0.01f);
                CheckSurrounding next_check = obj.GetComponent<CheckSurrounding>();
                next_check.CheckAdjascentFloor(object_grid, grid, boxes_placed);
            }
        }
    }
}
