using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateGrid : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp("escape")) Application.Quit();
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    public void Restart(Room room)
    {
        room.stage = Stage.grid;
        // When restarting pass existing entrance, and exit edges to ensure it matches existing maze layout
        StartCoroutine(CombineTemplates(room));
    }

    public void StartGenerating(Cell cell, Room room)
    {
        room.stage = Stage.grid;
        room.generated = false;
        room.entrance_dir = cell.entrance;
        room.exit_dirs = cell.exits;
        StartCoroutine(CombineTemplates(room));
    }

    private IEnumerator CombineTemplates(Room room)
    { 
        room.grid = new int[room.grid_x, room.grid_y];
        int[,] temp_grid = new int[room.grid_x, room.grid_y];
        int i = 0, x_pos = 0, y_pos = 0;
        while (i < room.num_templates)
        {
            temp_grid = room.grid;
            // Gets random template from list of templates
            int[,] template = GetComponent<Templates>().templates[(Random.Range(0, 17))].template;

            ManipulateTemp manipulate = new ManipulateTemp();

            // Rotate a random number of times
            int num_rotations = Random.Range(0, 4);
            if (num_rotations > 0)
            {
                for (int r = 0; r < num_rotations; r++)
                {
                    template = manipulate.Rotate(template);
                }
            }
            // 50% chance to flip template
            bool flip_template = Random.Range(0, 1) >= 0.5;
            if (flip_template) template = manipulate.Flip(template);

            // Start applying template
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    //Debug.Log("X:" + (x + x_pos).ToString() + " Y:" + (y + y_pos).ToString());
                    if (temp_grid[x + x_pos, y + y_pos] == 0 || temp_grid[x + x_pos, y + y_pos] == template[x, y])
                    {
                        temp_grid[x + x_pos, y + y_pos] = template[x, y];
                    }
                }
            }

            // Go to next position
            room.grid = temp_grid;
            i++;
            if (IsMultipleOf(i, room.size_x))
            {
                x_pos = 0;
                y_pos += 3;
            }
            else
            {
                x_pos += 3;
            }
            yield return null;
        }

        // Place walls around the generated room
        for (int y = 0; y < room.grid_y; y++)
        {
            for (int x = 0; x < room.grid_x; x++)
            {
                if (x == 0 || x == room.grid_x - 1 || y == 0 || y == room.grid_y - 1)
                {
                    room.grid[x, y] = 2;
                }
                yield return null;
            }
        }

        CheckGrid(room);
    }

    private bool IsMultipleOf(int x, int n)
    {
        // Returns the remainder after dividing x by n which will always be 0 if x is divisible by n.
        return (x % n) == 0;
    }

    private void CheckGrid(Room room)
    {
        GridCheck check = new GridCheck(room.grid);
        // If all checks are passed continue to next step, otherwise combine templates into new grid
        if (check.FloorCount())
        {
            if (check.ContinuousFloor())
            {
                // If all checks are passed continue to next step
                room.grid = check.FillGaps();
                StartCoroutine(PlaceDoorways(room));
            }
            else
            {
                Restart(room);
            }
        }
        else
        {
            Restart(room);
        }
    }


    IEnumerator PlaceDoorways(Room room)
    {
        room.exits = new List<Pos>();
        if (PlaceDoorway(room.entrance_dir, Elements.entrance, room))
        {
            for (int i = 0; i < room.exit_dirs.Count; i++)
            {
                if (!PlaceDoorway(room.exit_dirs[i], Elements.exit, room))
                {
                    Restart(room);
                    yield break;
                }
                yield return null;
            }
            GetComponent<GeneratePuzzle>().Generate(room);
        }
        else
        {
            Restart(room);
        }
    }

    bool PlaceDoorway(Direction edge, Elements type, Room room)
    {
        int x = 0, y = 0;
        switch (edge)
        {
            case Direction.N:
                {
                    x = (int)(room.grid.GetLength(0) / 2); 
                    y = room.grid.GetLength(1) - 1;
                    room.grid[x, y] = (int)type;
                    if (room.grid[x, y - 1] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.E:
                {
                    x = room.grid.GetLength(0) - 1;
                    y = (int)(room.grid.GetLength(1) / 2);
                    room.grid[x, y] = (int)type;
                    if (room.grid[x - 1, y] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.S:
                {
                    x = (int)(room.grid.GetLength(0) / 2); 
                    y = 0;
                    room.grid[x, y] = (int)type;
                    if (room.grid[x, y + 1] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.W:
                {
                    x = 0;
                    y = (int)(room.grid.GetLength(1) / 2);
                    room.grid[x, y] = (int)type;
                    if (room.grid[x + 1, y] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
        }

        if (type == Elements.entrance)
        {
            room.entrance = new Pos { x = x, y = y };
        }
        else if (type == Elements.exit)
        {
            room.exits.Add( new Pos { x = x, y = y } );
        }
        return true;
    }
}
