using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using enums;
public class GameControl : MonoBehaviour
{
    public int size_x = 3, size_y = 3, maze_x = 3, maze_y = 3, max_depth, steps_back, max_rooms;
    int grid_x { get { return (size_x * 3) + 2; } }
    int grid_y { get { return (size_y * 3) + 2; } }
    public bool show_dead_squares = false;
    public bool game_started = false;
    public float seconds = 0;
    public GameObject room_prefab;
    bool time_written = false;
    float time = 0;
    public Level this_level;
    public bool level_won = false;
    public bool generating = false;
    public bool PlayWhileGenerating = false, hide_rooms = false;
    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        if (!generating)
        {
            StartCoroutine(GenerateLevel());
        }
    }

    public IEnumerator GenerateLevel()
    {
        generating = true;
        game_started = false;
        if (this_level != null && this_level.level_object != null)
        {
            Destroy(this_level.level_object);
        }
        GameObject level_object = new GameObject();
        level_object.name = "Level";
        GenerateMaze maze_generator = GetComponent<GenerateMaze>();
        List<Cell> cells = maze_generator.GetMaze(new Cell[maze_x, maze_y], max_depth, steps_back, max_rooms);
        this_level = new Level { room_grid = new Room[maze_x, maze_y], object_grid = new GameObject[maze_x * grid_x, maze_y * grid_y], level_object = level_object };
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
            this_level.room_grid[current_cell.pos.x, current_cell.pos.y].room_object.transform.parent = this_level.level_object.transform;

            GenerateGrid generator = new GenerateGrid();
            generator.Generate(current_cell, this_level.room_grid[current_cell.pos.x, current_cell.pos.y]);

            GetComponent<GenerateObjects>().Generate(this_level.room_grid[current_cell.pos.x, current_cell.pos.y], this_level, show_dead_squares, hide_rooms);

            if (!game_started)
            {
                game_started = true;
                UpdateMove();
            }
            yield return null;
        }
        yield return new WaitForSeconds(1);
        generating = false;
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }



    public void Win()
    {
        level_won = true;
        DeleteLevel();
    }

    void DeleteLevel()
    {
        level_won = false;
        Generate();
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
            this_level.player.transform.position = new Vector3(pos.x, 1, pos.y);

            Debug.Log(this_level.boxes.Count.ToString());
            for (int i = 0; i < this_level.boxes.Count; i++)
            {
                if (this_level.moves[this_level.moves.Count - 1].box_pos.Count > i)
                {
                    pos = this_level.moves[this_level.moves.Count - 1].box_pos[i];
                    this_level.boxes[i].transform.parent = this_level.object_grid[pos.x, pos.y].transform;
                    this_level.boxes[i].transform.position = new Vector3(pos.x, 1, pos.y);
                }
            }
        }
    }
}
