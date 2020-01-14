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
    //List<GridTemplate> templates = new List<GridTemplate>();
    public int num_templates = 0;
    private GameObject[,] object_grid;
    private int[,] grid;
    private bool selected = false;
    public List<GameObject> floor_tiles = new List<GameObject>();
    [HideInInspector]  public List<GameObject> checked_floors = new List<GameObject>();
    public void Start()
    {
        num_templates = size_x * size_y;
        QualitySettings.vSyncCount = 0;
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        SetupGrid();
    }

    public void Restart()
    {
        selected = false;
        num_instances = 0;
        SetupGrid();
    }

    private void SetupGrid()
    {
        StopAllCoroutines();
        floor_tiles.Clear();
        checked_floors.Clear();
        StartCoroutine(LoadTemplates());
    }

    IEnumerator LoadTemplates()
    {
        GridTemplate[] templates = new GridTemplate[num_templates];
        for (int i = 0; i < num_templates; i++)
        {
            templates[i] = new GridTemplate(this.gameObject);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        grid = new int[grid_x, grid_y];
        StartApplying(templates);
    }

    void StartApplying(GridTemplate[] templates)
    {
        StopAllCoroutines();
        StartCoroutine(ApplyTemplate(0, 0, 0, 0, templates));
    }

    IEnumerator ApplyTemplate(int temp_num, int x_offset, int y_offset, int reset_count, GridTemplate[] templates)
    {
        Debug.Log("TEMP NUM: " + temp_num.ToString() + ", NUM TEMPLATES: " + templates.Length.ToString());
        Debug.Log("X_OFFSET: " + x_offset.ToString() + " Y_OFFSET: " + y_offset.ToString());

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
                        else if (grid[check_x, check_y] != templates[temp_num].GetTemplate(x, y))
                        {
                            Debug.Log("FAILED ON " + x.ToString() + " " + y.ToString());
                            SetupGrid();
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

        yield return new WaitForSeconds(0.1f);
        if (temp_num >= num_templates)
        {
            StartFilling();
            yield break;
        }
        else
        {
            StartCoroutine(ApplyTemplate(temp_num + 1, x_offset, y_offset, reset_count, templates));
        }
    }

    void StartFilling()
    {
        StopAllCoroutines();
        StartCoroutine(FillGaps());
    }

    IEnumerator FillGaps()
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
                StartCoroutine(CreateGrid());
                yield break;
            }
            yield return null;
        }
        StartCoroutine(CreateGrid());
    }

    IEnumerator CreateGrid()
    {
        if (object_grid != null)
        {
            for (int i = 0; i < object_grid.GetLength(0); i++)
            {
                for (int j = 0; j < object_grid.GetLength(1); j++)
                {
                    Destroy(object_grid[i, j]);
                    yield return null;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        object_grid = new GameObject[grid_x, grid_y];
        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                if (grid[x, y] == 1)
                {
                    object_grid[x, y] = Instantiate(floor_prefab, new Vector3(x, 0, y), Quaternion.identity);
                    floor_tiles.Add(object_grid[x, y]);
                }
                else if (grid[x, y] == 2)
                {
                    object_grid[x, y] = Instantiate(wall_prefab, new Vector3(x, 1, y), Quaternion.identity);
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);
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
        checked_floors.Add(tile);
    }

    public GameObject GetTile(int x, int y)
    {
        if (object_grid != null) return object_grid[x, y];
        else return null;
    }
}
