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
    public Level this_level;
    public bool level_won = false;

    private void Start()
    {
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        if (this_level != null && this_level.room_grid != null)
        {
            for (int y = 0; y < this_level.room_grid.GetLength(1); y++)
            {
                for (int x = 0; x < this_level.room_grid.GetLength(0); x++)
                {
                    Destroy(this_level.room_grid[x, y].room_object);
                    yield return null;
                }
            }
        }

        game_started = false;
        GenerateMaze maze_generator = GetComponent<GenerateMaze>();
        List<Cell> cells = maze_generator.GetMaze(new Cell[maze_x, maze_y]);
        this_level = new Level { room_grid = new Room[maze_x, maze_y], object_grid = new GameObject[maze_x * grid_x, maze_y * grid_y] };
        while (cells.Count > 0)
        {
            Cell current_cell = cells[0];
            cells.Remove(current_cell);
            this_level.room_grid[current_cell.pos.x, current_cell.pos.y] = new Room
            {
                pos = current_cell.pos,
                parent_level = this_level,
                room_object = Instantiate(room_prefab, new Vector3(current_cell.pos.x * grid_x, 0, current_cell.pos.y * grid_y), Quaternion.identity),
                offset_x = grid_x * current_cell.pos.x,
                offset_y = grid_y * current_cell.pos.y,
                first = current_cell.first_room,
                last = current_cell.last_room,
                num_boxes = (int)Mathf.Ceil((size_x * size_y) / 3.0f),
                num_templates = size_x * size_y,
                size_x = size_x,
                size_y = size_y,
                grid_x = grid_x,
                grid_y = grid_y
            };

            GenerateGrid generator = new GenerateGrid();
            generator.Generate(current_cell, this_level.room_grid[current_cell.pos.x, current_cell.pos.y]);
            while (!this_level.room_grid[current_cell.pos.x, current_cell.pos.y].generated)
            {
                yield return null;
            }

            GetComponent<GenerateObjects>().Generate(this_level.room_grid[current_cell.pos.x, current_cell.pos.y], this_level);
            while (!this_level.room_grid[current_cell.pos.x, current_cell.pos.y].instantiated)
            {
                yield return null;
            }

            if (!game_started)
            {
                game_started = true;
            }

        }
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
