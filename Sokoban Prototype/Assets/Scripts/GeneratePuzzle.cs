using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GeneratePuzzle : MonoBehaviour
{
    public int num_boxes = 3, min_steps = 8, max_attempts = 10;
    int[,] empty_grid;
    int attempts = 0;
    List<Pos> button_positions;
    List<Pos> box_positions;
    public void Generate(int[,] grid)
    {
        attempts = 0;
        empty_grid = grid.Clone() as int[,];
        PlaceButtons(grid.Clone() as int[,]);
    }

    private void PlaceButtons(int[,] grid)
    {
        // If number of attempts exceeds maximum number of attempts for a generated room generate a new empty room
        if (attempts >= max_attempts)
        {
            Debug.Log("MAX ATTEMPTS");
            GetComponent<GenerateGrid>().Restart();
            return;
        }

        button_positions = new List<Pos>();
        // Place buttons in valid floor tile positions
        while (button_positions.Count < num_boxes)
        {
            int x_pos = Random.Range(1, grid.GetLength(0) - 1);
            int y_pos = Random.Range(1, grid.GetLength(1) - 1);
            if (grid[x_pos, y_pos] == (int)Elements.floor)
            {
                grid[x_pos, y_pos] += (int)Elements.button;
                button_positions.Add(new Pos { x = x_pos, y = y_pos });
            }
        }
        Debug.Log("Buttons Placed");
        attempts++;
        StartCoroutine(PlaceBoxes(grid));
    }
    
    private IEnumerator PlaceBoxes(int[,] grid)
    {
        box_positions = new List<Pos>();
        int box = 0;
        Direction dir = Direction.N;
        // Continue looping until the required number of boxes are placed
        while (box_positions.Count < num_boxes)
        {
            int num_fails = 0;
            List<Pos> stepped_positions = new List<Pos>();
            stepped_positions.Add(button_positions[box]);
            // Continue looping until box position is far enough away
            while (stepped_positions.Count < min_steps)
            {
                dir = RandomDir();
                bool stepped = false;
                // Loop through all directions (unless valid direction is found)
                for (int i = 0; i < 4; i++)
                {
                    // Debug.Log("STEPPED COUNT " + stepped_positions.Count.ToString());
                    Pos checked_pos = CheckDir(dir, grid, stepped_positions[stepped_positions.Count - 1]);
                    // Check if CheckDir function passed (valid floor tile)
                    if (!checked_pos.empty)
                    {
                        // Check if the desired position has already been stepped on (if it has check another direction)
                        bool stepped_before = false;
                        for (int j = 0; j < stepped_positions.Count; j++)
                        {
                            if (checked_pos.x == stepped_positions[j].x && checked_pos.y == stepped_positions[j].y)
                            {
                                stepped_before = true;
                                break;
                            }
                        }
                        // If both direction checks pass, continue to next tile
                        if (!stepped_before)
                        {
                            stepped_positions.Add(checked_pos);
                            stepped = true;
                            break;
                        }
                    }
                    if (dir == Direction.W) dir = Direction.N;
                    else dir++;
                }

                yield return null;
                // If all directions have been attempted step backwards
                if (!stepped)
                {
                    num_fails++;
                    if (stepped_positions.Count > 1)
                    {
                        stepped_positions.Remove(stepped_positions[stepped_positions.Count - 1]);
                    }
                }

                // If failed too many times, restart goal placement (prevents getting stuck in loop going back and forth)
                if (num_fails >= 50)
                {
                    PlaceButtons(empty_grid.Clone() as int[,]);
                    yield break;
                }
            }

            // Once final position is found, add box position
            box++;
            grid[stepped_positions[stepped_positions.Count - 1].x, stepped_positions[stepped_positions.Count - 1].y] += (int)Elements.box;
            box_positions.Add(stepped_positions[stepped_positions.Count - 1]);
           
        }
        Debug.Log("Finished Box Placement");

        PlacePlayer(dir, grid);
    }


    void PlacePlayer(Direction dir, int[,]grid)
    {
        Pos player_pos = new Pos();
        //Spawns the player next to the last placed box

        if (dir == Direction.N)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x;
            player_pos.y = box_positions[box_positions.Count - 1].y + 1;
        }
        if (dir == Direction.E)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x + 1;
            player_pos.y = box_positions[box_positions.Count - 1].y;
        }
        if (dir == Direction.S)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x;
            player_pos.y = box_positions[box_positions.Count - 1].y - 1;
        }
        if (dir == Direction.W)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x - 1;
            player_pos.y = box_positions[box_positions.Count - 1].y;
        }

        grid[player_pos.x, player_pos.y] += (int)Elements.player;


        GetComponent<GenerateObjects>().Generate(grid);
    }

    private Direction RandomDir() { return (Direction)Random.Range(0, 4); }
    private Pos CheckDir(Direction dir, int[,] grid, Pos pos)
    {
        // Checks if tile in direction is empty floor
        if (dir == Direction.N && grid[pos.x, pos.y + 1] == (int)Elements.floor &&
            grid[pos.x, pos.y + 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y + 1 };
        }
        if (dir == Direction.E && grid[pos.x + 1, pos.y] == (int)Elements.floor && 
            grid[pos.x + 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x + 1, y = pos.y };
        }
        if (dir == Direction.S && grid[pos.x, pos.y - 1] == (int)Elements.floor && 
            grid[pos.x, pos.y - 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y - 1 };
        }
        if (dir == Direction.W && grid[pos.x - 1, pos.y] == (int)Elements.floor && 
            grid[pos.x - 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x - 1, y = pos.y };
        }
        return new Pos { empty = true };
    }
}
