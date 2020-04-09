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

        this_level.player = GameObject.FindGameObjectWithTag("Player");
        foreach (GameObject box in GameObject.FindGameObjectsWithTag("Box"))
        {
            this_level.box.Add(box);
        }

        UpdateMove();
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

    public void UpdateMove()
    {
        Pos player_pos = new Pos() { x = (int)this_level.player.transform.parent.position.x, y = (int)this_level.player.transform.parent.position.z };

        Debug.Log(this_level.box.Count.ToString());
        List<Pos> box_pos = new List<Pos>();
        for (int i = 0;  i < this_level.box.Count; i++)
        {
            box_pos.Add(new Pos() { x = (int)this_level.box[i].transform.parent.position.x, y = (int)this_level.box[i].transform.parent.position.z });
        }

        this_level.moves.Add(new Move { player_pos = player_pos, box_pos = box_pos });
    }

    public void StepBack()
    {
        Debug.Log("Num moves: " + this_level.moves.Count.ToString());
        if (this_level.moves.Count > 1)
        {
            this_level.moves.RemoveAt(this_level.moves.Count - 1);
            Pos pos = this_level.moves[this_level.moves.Count - 1].player_pos;
            this_level.player.transform.parent = this_level.object_grid[pos.x, pos.y].transform;
            this_level.player.transform.position = new Vector3(pos.x, 0.6f, pos.y);

            for (int i = 0; i < this_level.box.Count; i++)
            {
                pos = this_level.moves[this_level.moves.Count - 1].box_pos[i];
                this_level.box[i].transform.parent = this_level.object_grid[pos.x, pos.y].transform;
                this_level.box[i].transform.position = new Vector3(pos.x, 0.5f, pos.y);
            }
        }
    }
}
