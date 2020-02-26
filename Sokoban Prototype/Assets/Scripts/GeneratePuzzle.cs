using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GeneratePuzzle : MonoBehaviour
{
    public int num_boxes = 3, min_steps = 8;
    int[,] room_grid;

    public void Generate(int[,] grid)
    {
        room_grid = grid;
        PlaceButtons();
    }

    private void PlaceButtons()
    {
        // Place buttons in valid floor tile positions
        List<int[]> button_positions = new List<int[]>();
        while (button_positions.Count < num_boxes)
        {
            int x = Random.Range(1, room_grid.GetLength(0) - 1);
            int y = Random.Range(1, room_grid.GetLength(1) - 1);

            if (room_grid[x,y] == 1)
            {
                button_positions.Add(new int[2] { x, y });
            }
        }
        Debug.Log("Buttons Placed");
        StartCoroutine(PlaceBoxes(button_positions));
    }
    
    private IEnumerator PlaceBoxes(List<int[]> button_positions)
    {
        List<int[]> box_positions = new List<int[]>();
        int box = 0;
        // Continue looping until the required number of boxes are placed
        while (box_positions.Count < num_boxes)
        {
            List<int[]> stepped_positions = new List<int[]>();
            stepped_positions.Add(button_positions[box]);
            // Continue looping until box position is far enough away
            while (stepped_positions.Count < min_steps)
            {
                Direction dir = RandomDir();
                bool stepped = false;
                // Loop through all directions (unless valid direction is found)
                for (int i = 0; i < 4; i++)
                {
                    //Debug.Log("STEPPED COUNT " + stepped_positions.Count.ToString());
                    int[] checked_pos = CheckDir(dir, stepped_positions[stepped_positions.Count - 1]);
                    // Check if CheckDir function passed (valid floor tile)
                    if (checked_pos[0] != 0 && checked_pos[1] != 0)
                    {
                        // Check if the desired position has already been stepped on (if it has check another direction)
                        bool stepped_before = false;
                        for (int j = 0; j < stepped_positions.Count; j++)
                        {
                            if (checked_pos[0] == stepped_positions[j][0] && checked_pos[1] == stepped_positions[j][1])
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
                    stepped_positions.Remove(stepped_positions[stepped_positions.Count - 1]);
                }
            }
            // Once final position is found, add box position
            box++;
            box_positions.Add(stepped_positions[stepped_positions.Count - 1]);
        }
        Debug.Log("Finished Box Placement");
        GetComponent<GenerateObjects>().Generate(room_grid, button_positions, box_positions);
    }

    private Direction RandomDir() { return (Direction)Random.Range(0, 4); }
    private int[] CheckDir(Direction dir, int[] pos)
    {
        if (dir == Direction.N && room_grid[pos[0], pos[1] + 1] == 1 &&
            room_grid[pos[0], pos[1] + 2] == 1)
        {
            return new int[2] { pos[0], pos[1] + 1 };
        }
        if (dir == Direction.E && room_grid[pos[0] + 1, pos[1]] == 1 && 
            room_grid[pos[0] + 2, pos[1]] == 1)
        {
            return new int[2] { pos[0] + 1, pos[1] };
        }
        if (dir == Direction.S && room_grid[pos[0], pos[1] - 1] == 1 && 
            room_grid[pos[0], pos[1] - 2] == 1)
        {
            return new int[2] { pos[0], pos[1] - 1 };
        }
        if (dir == Direction.W && room_grid[pos[0] - 1, pos[1]] == 1 && 
            room_grid[pos[0] - 2, pos[1]] == 1)
        {
            return new int[2] { pos[0] - 1, pos[1] };
        }
        return new int[2] { 0, 0 };
    }
}
