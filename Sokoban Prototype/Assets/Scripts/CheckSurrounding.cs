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
    public void CheckAdjascentFloor (List<GameObject> floor_tiles, GameObject grid)
    {
        GenerateGrid grid_script = grid.GetComponent<GenerateGrid>();
        foreach (GameObject obj in floor_tiles)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) == 1 && !grid_script.checked_floors.Contains(obj) && 
                (obj.transform.childCount == 0 || !obj.transform.GetChild(0).CompareTag("Box")))
            {
                grid_script.AddToChecked(obj);
                CheckSurrounding next_check = obj.GetComponent<CheckSurrounding>();
                next_check.CheckAdjascentFloor(floor_tiles, grid);
            }
        }
    }
}
