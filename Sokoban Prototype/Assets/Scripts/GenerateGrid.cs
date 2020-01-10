using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    private int max_instances = 64;
    private int num_instances = 0;
    public int size_x = 3;
    public int size_y = 3;
    public GameObject floor_prefab;
    public GameObject wall_prefab;
    private GridTemplate[,] template_grid;
    private GameObject[,] object_grid;
    private int[,] grid;
    private bool selected = false;
    private List<GameObject> floor_tiles = new List<GameObject>();
    [HideInInspector]  public List<GameObject> checked_floors = new List<GameObject>();
    public void Start()
    {
        StartCoroutine(SetupGrid());
    }

    private IEnumerator SetupGrid()
    {
        if (selected || num_instances >= max_instances) yield break;

        if (object_grid != null)
        {
            floor_tiles.Clear();
            checked_floors.Clear();
        }

        num_instances++;
 
        template_grid = new GridTemplate[size_x, size_y];
        for (int y = 0; y < size_y; y++)
        {
            for (int x = 0; x < size_x; x++)
            {
                template_grid[x, y] = new GridTemplate(this.gameObject);
            }
        }

        int[,] temp_grid = new int[size_x *3, size_y * 3];
        int x_offset = -1, y_offset = -1;

        for (int y = 0; y < size_x; y++)
        {
            for (int x = 0; x < size_y; x++)
            {
                for (int iy = 0; iy < 5; iy++)
                {
                    for (int ix = 0; ix < 5; ix++)
                    {
                        int check_x = x_offset + ix;
                        int check_y = y_offset + iy;
                        if (check_x >= 0 && check_y >= 0 && check_x < size_x * 3 && check_y < size_y * 3)
                        {
                            if (temp_grid[check_x, check_y] == 0)
                            {                                
                                temp_grid[check_x, check_y] = template_grid[x, y].GetTemplate(ix, iy);
                            }
                            if (temp_grid[check_x, check_y] != template_grid[x, y].GetTemplate(ix, iy))
                            {
                                // Restart
                                StartCoroutine(SetupGrid());
                            }
                        }
                        yield return null;
                    }
                }
                x_offset += 3;
            }
            x_offset = -1;
            y_offset += 3;
        }

        if (selected)
        {
            num_instances--;
            yield break;
        }

        selected = true;
        grid = temp_grid;
        CreateGrid();
    }

    private void CreateGrid()
    {
        if (object_grid != null)
        {
            for (int i = 0; i < object_grid.GetLength(0); i++)
            {
                for (int j = 0; j < object_grid.GetLength(1); j++)
                {
                    Destroy(object_grid[i, j]);
                }
            }
        }

        object_grid = new GameObject[size_x * 3, size_y * 3];
        for (int x = 0; x < size_x * 3; x++)
        {
            for (int y = 0; y < size_y * 3; y++)
            {
                if (grid[x, y] == 1)
                {
                    floor_tiles.Add(object_grid[x,y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity));
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                }
            }
        }

        StartCoroutine(StartFloorChecks());
    }


    IEnumerator StartFloorChecks()
    {
        CheckSurrounding floor_check = floor_tiles[0].GetComponent<CheckSurrounding>();
        floor_check.CheckAdjascent(floor_tiles, this.gameObject);
        yield return new WaitForSeconds(0.1f);
        if (checked_floors.Count < floor_tiles.Count)
        {
            Debug.Log("Floor Not Continuous - Discarding Layout...");
            selected = false;
            num_instances = 0;
            StartCoroutine(SetupGrid());
        }
        else
        {
            Debug.Log("Continuous Floor");
            // Continue to next layout check
        }
    }

    public void AddToChecked(GameObject tile)
    {
        checked_floors.Add(tile);
    }
}
