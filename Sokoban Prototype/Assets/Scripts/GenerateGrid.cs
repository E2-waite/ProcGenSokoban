using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    public int size_x = 3, size_y = 3;
    int grid_x = 0, grid_y = 0;
    public int max_instances = 128;
    int failed = 0;
    public GameObject floor_prefab, wall_prefab, grid_prefab;
    bool[,] floor_grid;
    GameObject grid_obj;
    public int num_templates = 0;
    private GameObject[,] object_grid;
    public List<GameObject> floor_tiles = new List<GameObject>();
    [HideInInspector]  public List<GameObject> checked_floors = new List<GameObject>();
    bool template_selected = false;
    public void Start()
    {
        num_templates = size_x * size_y;
        QualitySettings.vSyncCount = 0;
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        SetupGrid();
    }

    private void Update()
    {
        if (failed >= max_instances && !template_selected)
        {
            failed = 0;
            SetupGrid();
        }
    }

    public void Restart()
    {
        SetupGrid();
    }

    private void SetupGrid()
    {
        Debug.Log("RESTARTING");
        template_selected = false;
        if (grid_obj != null) Destroy(grid_obj);
        StopAllCoroutines();
        failed = 0;
        StartCoroutine(LoadTemplates());
    }

    IEnumerator LoadTemplates()
    {
        for (int i = 0; i < max_instances; i++)
        {
            GridTemplate[] templates = new GridTemplate[num_templates];
            for (int j = 0; j < num_templates; j++)
            {
                templates[j] = new GridTemplate(this.gameObject);
                yield return null;
            }

            int[,] grid = new int[grid_x, grid_y];
            StartCoroutine(ApplyTemplate(0, 0, 0, 0, templates, grid));
        }
    }

    IEnumerator ApplyTemplate(int temp_num, int x_offset, int y_offset, int reset_count, GridTemplate[] templates, int[,] grid)
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                int check_x = x_offset + x;
                int check_y = y_offset + y;
                if (check_x >= 0 && check_y >= 0 && check_x < grid_x && check_y < grid_y)
                {
                    if (check_x == 0 || check_y == 0 || check_x == grid_x - 1 || check_y == grid_y - 1)
                    {
                        grid[check_x, check_y] = 2;
                    }
                    else
                    {
                        if (grid[check_x, check_y] == 0)
                        {
                            grid[check_x, check_y] = templates[temp_num].GetTemplate(x, y);
                        }
                        else if (grid[check_x, check_y] != templates[temp_num].GetTemplate(x, y) && templates[temp_num].GetTemplate(x, y) != 0)
                        {
                            failed++;
                            yield break;
                        }
                    }
                }
                yield return null;
            } 
        }
         
        if (x_offset == 3 * (size_x - 1))
        {
            x_offset = 0;
            y_offset += 3;
        }
        else x_offset += 3;

        if (temp_num >= num_templates - 1)
        {
            StartFilling(grid);
            yield break;
        }
        else
        {
            StartCoroutine(ApplyTemplate(temp_num + 1, x_offset, y_offset, reset_count, templates, grid));
        }
    }

    void StartFilling(int[,] grid)
    {
        template_selected = true;
        StopAllCoroutines();
        StartCoroutine(FillGaps(grid));
    }

    IEnumerator FillGaps(int[,] grid) 
    {
        Debug.Log("FILLING");
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
                StartFloorCheck(grid);
                yield break;
            }
            yield return null;
        }
        StartFloorCheck(grid);
    }

    void StartFloorCheck(int[,] grid)
    {
        StopAllCoroutines();
        StartCoroutine(FloorCheck(grid));
    }

    IEnumerator FloorCheck(int[,] grid)
    {
        Debug.Log("STARTING FLOOR CHECK");
        int num_floors = 0;
        floor_grid = new bool[grid_x, grid_y];
        List<int[]> checked_floors = new List<int[]>();

        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    if (checked_floors.Count == 0) checked_floors.Add(new int[2] { x, y });
                    num_floors++;
                }
                yield return null;
            }
        }

        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    if (x == 0 && y == 0) Debug.Log("FIRST");
                    else
                    {
                        for (int i = 0; i < checked_floors.Count; i++)
                        {
                            if ((checked_floors[i][0] == x + 1 && checked_floors[i][1] == y) || (checked_floors[i][0] == x - 1 && checked_floors[i][1] == y) ||
                                (checked_floors[i][0] == x && checked_floors[i][1] == y + 1) || (checked_floors[i][0] == x && checked_floors[i][1] == y - 1))
                            {
                                checked_floors.Add(new int[2] { x, y });
                                break;
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(1);

        Debug.Log("CHECKED FLOORS: " + checked_floors.Count.ToString());
        if (checked_floors.Count == num_floors)
        {
            StartGenerating(grid);
        }
        else
        {
            Debug.Log("FAILED FLOOR CHECK, NUM_FLOORS: " + num_floors.ToString() + " CHECKED FLOORS: " + checked_floors.Count.ToString());
            SetupGrid();
        }
    }

    void StartGenerating(int[,] grid)
    {
        StopAllCoroutines();
        StartCoroutine(CreateGrid(grid));
    }

    IEnumerator CreateGrid(int[,] grid)
    {
        if (grid_obj == null)
        {
            grid_obj = Instantiate(grid_prefab, transform);
        }

        yield return new WaitForSeconds(0.1f);
        floor_tiles.Clear();
        object_grid = new GameObject[grid_x, grid_y];
        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    object_grid[x, y].transform.parent = grid_obj.transform;
                    floor_tiles.Add(object_grid[x, y]);
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                    object_grid[x, y].transform.parent = grid_obj.transform;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);
        //FloorCheck(false, 0);
    }

    public void FloorCheck(bool boxes_placed, int num_boxes)
    {
        StopAllCoroutines();
        checked_floors.Clear();
        StartCoroutine(StartFloorChecks(boxes_placed, num_boxes));
    }

    IEnumerator StartFloorChecks(bool boxes_placed, int num_boxes)
    {
        if (boxes_placed) Debug.Log("Started post-box floor check" + floor_tiles.Count.ToString());
        CheckSurrounding floor_check = floor_tiles[0].GetComponent<CheckSurrounding>();
        floor_check.CheckAdjascentFloor(object_grid, this.gameObject, boxes_placed);
        yield return new WaitForSeconds(0.5f);
        PlaceGoals goals = GetComponent<PlaceGoals>();
        if (checked_floors.Count < floor_tiles.Count - num_boxes)
        {
            Debug.Log("FLOOR CHECK FAILED, FLOOR TILES: " + floor_tiles.Count.ToString() + ", CHECKED FLOORS: " + checked_floors.Count.ToString());
            if (boxes_placed)
            {
                goals.NextAttempt();
                yield break;
            }
            else SetupGrid();
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
        if (!checked_floors.Contains(tile)) checked_floors.Add(tile);
    }

    public GameObject GetTile(int x, int y)
    {
        if (object_grid != null) return object_grid[x, y];
        else return null;
    }
}
