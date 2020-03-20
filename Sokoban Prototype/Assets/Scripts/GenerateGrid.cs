using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateGrid : MonoBehaviour
{
    public int size_x = 3, size_y = 3;
    int grid_x = 0, grid_y = 0, num_templates;
    Direction entrance_edge = Direction.None, exit_edge = Direction.None;

    private void Update()
    {
        if (Input.GetKeyUp("escape")) Application.Quit();
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        num_templates = size_x * size_y;
        StartGenerating(Direction.S, Direction.E);
    }

    public void Restart()
    {
        Debug.Log("NEW ROOM");
        // When restarting pass existing entrance, and exit edges to ensure it matches existing maze layout
        StartCoroutine(CombineTemplates());
    }

    public void StartGenerating(Direction entrance, Direction exit)
    {
        entrance_edge = entrance;
        exit_edge = exit;
        StartCoroutine(CombineTemplates());
    }

    private IEnumerator CombineTemplates()
    {
        int[,] grid = new int[grid_x, grid_y];
        int[,] temp_grid = new int[grid_x, grid_y];
        int i = 0, x_pos = 0, y_pos = 0;
        while (i < num_templates)
        {
            temp_grid = grid;
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
            grid = temp_grid;
            i++;
            if (IsMultipleOf(i, size_x))
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
        for (int y = 0; y < grid_y; y++)
        {
            for (int x = 0; x < grid_x; x++)
            {
                if (x == 0 || x == grid_x - 1 || y == 0 || y == grid_y - 1)
                {
                    grid[x, y] = 2;
                }
                yield return null;
            }
        }

        CheckGrid(grid);
    }

    private bool IsMultipleOf(int x, int n)
    {
        // Returns the remainder after dividing x by n which will always be 0 if x is divisible by n.
        return (x % n) == 0;
    }

    private void CheckGrid(int[,] grid)
    {
        GridCheck check = new GridCheck(grid);
        // If all checks are passed continue to next step, otherwise combine templates into new grid
        if (check.FloorCount())
        {
            if (check.ContinuousFloor(check.num_floors))
            {
                // If all checks are passed continue to next step
                PlaceDoorways(check.FillGaps());
            }
            else
            {
                Restart();
            }
        }
        else
        {
            Restart();
        }
    }

    private void PlaceDoorways(int[,] grid)
    {
        if (PlaceDoorway(entrance_edge, Elements.entrance, grid) && PlaceDoorway(exit_edge, Elements.exit, grid))
        {
            GetComponent<GeneratePuzzle>().Generate(grid);
        }
        else
        {
            Restart();
        }
    }

    bool PlaceDoorway(Direction edge, Elements type, int[,] grid)
    {
        switch (edge)
        {
            case Direction.N:
                {
                    int x = (int)(grid.GetLength(0) / 2), y = grid.GetLength(1) - 1;
                    grid[x, y] = (int)type;
                    if (grid[x, y - 1] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.E:
                {
                    int x = grid.GetLength(0) - 1, y = (int)(grid.GetLength(1) / 2);
                    grid[x, y] = (int)type;
                    if (grid[x - 1, y] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.S:
                {
                    int x = (int)(grid.GetLength(0) / 2), y = 0;
                    grid[x, y] = (int)type;
                    if (grid[x, y + 1] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.W:
                {
                    int x = 0, y = (int)(grid.GetLength(1) / 2);
                    grid[x, y] = (int)type;
                    if (grid[x + 1, y] == (int)Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
        }

        return true;
    }
}
