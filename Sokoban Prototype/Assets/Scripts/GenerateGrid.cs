using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    private readonly int max_instances = 64;
    private int num_instances = 0;
    public int size_x = 3, size_y = 3;
    int grid_x = 0, grid_y = 0;
    public GameObject floor_prefab, wall_prefab;
    private GridTemplate[,] template_grid;
    private GameObject[,] object_grid;
    private int[,] grid;
    private bool selected = false;
    public List<GameObject> floor_tiles = new List<GameObject>();
    [HideInInspector]  public List<GameObject> checked_floors = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyUp("escape")) Application.Quit();
    }

    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        StartCoroutine(SetupGrid());
    }

    public void Restart()
    {
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
        StartCoroutine(FillGaps());
    }

    IEnumerator FillGaps()
    {
        // If a floor tile is surrounded by wall tiles (in 3 or more directions) fill in with wall tile
        int max_passes = 8;
        for (int i = 0; i < max_passes; i++)
        {
            int highest_walls = 0;
            for (int x = 0; x < grid_x; x++)
            {
                for (int y = 0; y < grid_y; y++)
                {
                    int surrounding_walls = 0;
                    if (grid[x, y] == 1)
                    {
                        if (grid[x + 1, y] == 2) surrounding_walls++;
                        if (grid[x - 1, y] == 2) surrounding_walls++;
                        if (grid[x, y + 1] == 2) surrounding_walls++;
                        if (grid[x, y - 1] == 2) surrounding_walls++;

                        if (surrounding_walls > highest_walls) highest_walls = surrounding_walls;

                        if (surrounding_walls >= 3)
                        {
                            grid[x, y] = 2;
                        }
                    }
                    yield return null;
                }
            }

            if (highest_walls < 3)
            {
                StartCoroutine(FloorCount());
                yield break;
            }
        }
        StartCoroutine(FloorCount());
    }

    IEnumerator FloorCount()
    {
        int num_floors = 0;
        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    num_floors++;
                }
                yield return null;
            }
        }

        if (num_floors < (grid_x * grid_y) / 4)
        {
            Debug.Log("FAILED FLOOR COUNT, NUM FLOORS: " + num_floors.ToString() + "/" + ((grid_x * grid_y) / 4).ToString());
            Restart();
        }
        else CreateGrid();
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
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                }
            }
        }

        StartCoroutine(StartFloorChecks(false, 0));
    }


    public IEnumerator StartFloorChecks(bool boxes_placed, int num_boxes)
    {
        if (boxes_placed) Debug.Log("Started post-box floor check" + floor_tiles.Count.ToString());
        CheckSurrounding floor_check = floor_tiles[0].GetComponent<CheckSurrounding>();
        checked_floors.Clear();
        floor_check.CheckAdjascentFloor(floor_tiles, this.gameObject);
        yield return new WaitForSeconds(0.1f);
        PlaceGoals goals = GetComponent<PlaceGoals>();
        if (checked_floors.Count < floor_tiles.Count - num_boxes)
        {
            selected = false;
            num_instances = 0;
            if (boxes_placed)
            {
                goals.NextAttempt();
                yield break;
            }
            else StartCoroutine(SetupGrid());
        }
        else
        {
            
            if (boxes_placed)
            {
                goals.StartCoroutine(goals.PlacePlayer());
                yield break;
            }
            else goals.StartPlacing();
        }    
    }

    public void AddToChecked(GameObject tile)
    {
        checked_floors.Add(tile);
    }

    public GameObject GetTile(int x, int y)
    {
        if (object_grid != null) return object_grid[x, y];
        else return null;
    }
}
