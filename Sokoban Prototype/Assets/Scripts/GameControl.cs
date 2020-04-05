using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using enums;
public class GameControl : MonoBehaviour
{
    public int size_x = 3, size_y = 3, maze_x = 3, maze_y = 3;
    int grid_x, grid_y;

    public bool game_started = false;
    public float seconds = 0;
    public GameObject room_prefab;

    bool time_written = false;
    float time = 0;
    public GameObject player;
    Level next_level = null;
    public Level this_level;
    public bool level_won = false;

    private void Start()
    {
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        this_level = GetComponent<GenerateLevel>().Generate(size_x, size_y, grid_x, grid_y, maze_x, maze_y);
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        while (!this_level.generated)
        {
            yield return null;
        }

        // Instantiate room object and instantiate objects within
        for (int y = 0; y < this_level.room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < this_level.room_grid.GetLength(0); x++)
            {
                this_level.room_grid[x, y].room_object = Instantiate(room_prefab, new Vector3(x * grid_x, 0, y * grid_y), Quaternion.identity);
                this_level.room_grid[x, y].room_object.transform.parent = transform;
                GetComponent<GenerateObjects>().Generate(this_level.room_grid[x, y]);
            }
        }
        this_level.instantiated = true;

        // Combine multiple room's object grids together to create a single object grid
        this_level.grid = new int[this_level.room_grid.GetLength(0) * grid_x, this_level.room_grid.GetLength(1) * grid_y];
        this_level.object_grid = new GameObject[this_level.room_grid.GetLength(0) * grid_x, this_level.room_grid.GetLength(1) * grid_y];
        for (int y = 0; y < this_level.room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < this_level.room_grid.GetLength(0); x++)
            {
                for (int iy = 0; iy < grid_y; iy++)
                {
                    for (int ix = 0; ix < grid_x; ix++)
                    {
                        this_level.grid[ix + (x * grid_x), iy + (y * grid_y)] = this_level.room_grid[x, y].grid[ix, iy];
                        this_level.object_grid[ix + (x * grid_x), iy + (y * grid_y)] = this_level.room_grid[x, y].object_grid[ix, iy];
                        this_level.object_grid[ix + (x * grid_x), iy + (y * grid_y)].name = (ix + (x * grid_x)).ToString() + " " + (iy + (y * grid_y)).ToString();
                    }
                }
            }
        }

        next_level = GetComponent<GenerateLevel>().Generate(size_x, size_y, grid_x, grid_y, maze_x, maze_y);
        game_started = true;
    }

    private void Update()
    {
        if (!game_started)
        {
            time += Time.deltaTime;
            seconds = time % 60;
        }
        else
        {
            if (!time_written)
            {
                time_written = true;
                WriteTime();
            }
        }

        if (level_won)
        {
            DeleteLevel();
        }
    }

    void WriteTime()
    {
        Debug.Log("Writing time");
        StreamWriter writer = new StreamWriter("Assets/GenerationTime.txt", true);
        writer.WriteLine(seconds.ToString());
        writer.Close();
    }

    void DeleteLevel()
    {
        level_won = false;
        for (int y = 0; y < this_level.room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < this_level.room_grid.GetLength(0); x++)
            {
                Destroy(this_level.room_grid[x, y].room_object);
            }
        }
        this_level = next_level;
        StartCoroutine(GenerateLevel());
    }

    public void UpdatePosition(Elements element, int[] old_pos, int[] new_pos)
    {
        this_level.grid[old_pos[0], old_pos[1]] -= (int)element;
        this_level.grid[new_pos[0], new_pos[1]] += (int)element;
    }
}
