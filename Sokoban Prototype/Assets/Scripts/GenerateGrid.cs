using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    private int max_instances = 64, num_instances = 0;
    public int size_x = 3, size_y = 3;
    int grid_x = 0, grid_y = 0;
    public GameObject floor_prefab, wall_prefab;
    private GridTemplate[,] template_grid;
    private GameObject[,] object_grid;
    private int[,] grid;
    private bool selected = false;
    public List<GameObject> floor_tiles = new List<GameObject>();
    private List<GameObject> tiles = new List<GameObject>();
    [HideInInspector]  public List<GameObject> checked_floors = new List<GameObject>();
    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        StartCoroutine(SetupGrid());
    }

    public void Restart()
    {
        //Debug.Log("RESTARTING");
        selected = false;
        num_instances = 0;
        StartCoroutine(SetupGrid());
    }

    private IEnumerator SetupGrid()
    {
        if (selected || num_instances >= max_instances) yield break;

        if (object_grid != null)
        {
            floor_tiles.Clear();
            checked_floors.Clear();
            tiles.Clear();
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

        int[,] temp_grid = new int[grid_x, grid_y];
        int x_offset = 0, y_offset = 0;

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
                        if (check_x >= 0 && check_y >= 0 && check_x < grid_x && check_y < grid_y)
                        {
                            if (check_x == 0 || check_y == 0 || check_x == grid_x - 1 || check_y == grid_y - 1)
                            {
                                temp_grid[check_x, check_y] = 2;
                            }
                            else
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
                        }
                        yield return null;
                    }
                }
                x_offset += 3;
            }
            x_offset = 0;
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

        object_grid = new GameObject[grid_x, grid_y];
        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    object_grid[x,y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    floor_tiles.Add(object_grid[x, y]);
                    tiles.Add(object_grid[x, y]);
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                    tiles.Add(object_grid[x, y]);
                }
            }
        }

        StartCoroutine(StartFloorChecks());
    }


    IEnumerator StartFloorChecks()
    {
        CheckSurrounding floor_check = floor_tiles[0].GetComponent<CheckSurrounding>();
        floor_check.CheckAdjascentFloor(floor_tiles, this.gameObject);
        yield return new WaitForSeconds(0.1f);
        if (checked_floors.Count < floor_tiles.Count)
        {
            //Debug.Log("Floor Not Continuous - Discarding Layout...");
            selected = false;
            num_instances = 0;
            StartCoroutine(SetupGrid());
        }
        else
        {
            //Debug.Log("Continuous Floor");
            //StartCoroutine(StartWallChecks());
            GetComponent<PlaceGoals>().StartPlacing();
        }    
    }

    IEnumerator StartWallChecks()
    {
        for (int i = 0; i < floor_tiles.Count; i++)
        {
            if (floor_tiles[i].GetComponent<CheckSurrounding>().CheckAdjascentWalls(tiles))
            {
                selected = false;
                num_instances = 0;
                StartCoroutine(SetupGrid());
                yield break;
            }
            yield return null;
        }
        Debug.Log("No Dead States Detected");
    }

    public void AddToChecked(GameObject tile)
    {
        checked_floors.Add(tile);
    }

    public GameObject GetTile(int x, int y)
    {
        return object_grid[x, y];
    }
}
