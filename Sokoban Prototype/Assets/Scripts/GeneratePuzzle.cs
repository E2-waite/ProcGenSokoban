using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneratePuzzle
{
    int num_boxes;
    int[,] room_grid;
    public GeneratePuzzle(int[,] grid, int boxes)
    {
        room_grid = grid;
        num_boxes = boxes;
        PlaceButtons();
    }

    void PlaceButtons()
    {
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
        PlaceBoxes(button_positions);
    }
    
    void PlaceBoxes(List<int[]> button_positions)
    {
        List<int[]> box_positions = new List<int[]>();
        while (box_positions.Count < num_boxes)
        {

        }
    }
}
