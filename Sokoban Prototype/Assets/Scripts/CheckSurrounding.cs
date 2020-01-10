using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSurrounding : MonoBehaviour
{
    public void CheckAdjascent (List<GameObject> floor_tiles, GameObject grid)
    {
        GenerateGrid grid_script = grid.GetComponent<GenerateGrid>();
        foreach (GameObject obj in floor_tiles)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance == 1 && !grid_script.checked_floors.Contains(obj))
            {
                grid_script.AddToChecked(obj);
                CheckSurrounding next_check = obj.GetComponent<CheckSurrounding>();
                next_check.CheckAdjascent(floor_tiles, grid);
            }
        }
    }
}
