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
        while (this_level.maze_cells.Count == 0)
        {
            yield return null;
        }

        this_level.object_grid = new GameObject[maze_x * grid_x, maze_y * grid_y];

        // Generate rooms, starting with the first room and continuing through in order of depth
        for (int i = 0; i < this_level.maze_cells.Count; i++)
        {
            while (!this_level.room_grid[this_level.maze_cells[i].GetPos().x, this_level.maze_cells[i].GetPos().y].generated)
            {
                yield return null;
            }
            this_level.room_grid[this_level.maze_cells[i].GetPos().x, this_level.maze_cells[i].GetPos().y].room_object = 
                Instantiate(room_prefab, new Vector3(this_level.maze_cells[i].GetPos().x * grid_x, 0, this_level.maze_cells[i].GetPos().y * grid_y), Quaternion.identity);
            this_level.room_grid[this_level.maze_cells[i].GetPos().x, this_level.maze_cells[i].GetPos().y].room_object.transform.parent = transform;
            GetComponent<GenerateObjects>().Generate(this_level.room_grid[this_level.maze_cells[i].GetPos().x, this_level.maze_cells[i].GetPos().y]);

            for (int y = 0; y < grid_y; y++)
            {
                for (int x = 0; x < grid_x; x++)
                {
                    Pos pos = new Pos { x = this_level.maze_cells[i].GetPos().x, y = this_level.maze_cells[i].GetPos().y };
                    this_level.object_grid[x + (pos.x * grid_x), y + (pos.y * grid_y)] = this_level.room_grid[pos.x, pos.y].object_grid[x, y];
                    this_level.object_grid[x + (pos.x * grid_x), y + (pos.y * grid_y)].name = (x + (pos.x * grid_x)).ToString() + " " + (y + (this_level.maze_cells[i].GetPos().y * grid_y)).ToString();
                    yield return null;
                }
            }

            if (i == 0)
            {
                game_started = true;
                UpdateMove();
            }
        }
        this_level.instantiated = true;

        //next_level = GetComponent<GenerateLevel>().Generate(size_x, size_y, grid_x, grid_y, maze_x, maze_y);
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
    }

    public void Win()
    {
        level_won = true;
        DeleteLevel();
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
        if (this_level.room_grid != null)
        {
            for (int y = 0; y < this_level.room_grid.GetLength(1); y++)
            {
                for (int x = 0; x < this_level.room_grid.GetLength(0); x++)
                {
                    Destroy(this_level.room_grid[x, y].room_object);
                }
            }
        }
        this_level = next_level;
        StartCoroutine(GenerateLevel());
    }

    public void UpdateMove()
    {
        Pos player_pos = new Pos() { x = (int)this_level.player.transform.parent.position.x, y = (int)this_level.player.transform.parent.position.z };
        List<Pos> box_pos = new List<Pos>();

        for (int i = 0;  i < this_level.boxes.Count; i++)
        {
            box_pos.Add(new Pos() { x = (int)this_level.boxes[i].transform.parent.position.x, y = (int)this_level.boxes[i].transform.parent.position.z });
        }

        this_level.moves.Add(new Move { player_pos = player_pos, box_pos = box_pos });
    }

    public void StepBack()
    {
        if (this_level.moves.Count > 1)
        {
            this_level.moves.RemoveAt(this_level.moves.Count - 1);
            Pos pos = this_level.moves[this_level.moves.Count - 1].player_pos;
            this_level.player.transform.parent = this_level.object_grid[pos.x, pos.y].transform;
            this_level.player.transform.position = new Vector3(pos.x, 0.6f, pos.y);

            for (int i = 0; i < this_level.boxes.Count; i++)
            {
                if (this_level.moves[this_level.moves.Count - 1].box_pos.Count > i)
                {
                    pos = this_level.moves[this_level.moves.Count - 1].box_pos[i];
                    this_level.boxes[i].transform.parent = this_level.object_grid[pos.x, pos.y].transform;
                    this_level.boxes[i].transform.position = new Vector3(pos.x, 0.5f, pos.y);
                }
            }
        }
    }
}
