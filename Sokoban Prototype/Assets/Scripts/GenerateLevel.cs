using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateLevel : MonoBehaviour
{
    public int size_x = 3, size_y = 3;
    public GameObject room_prefab;
    Room[,] room_grid;

    public void Generate(Cell[,] maze_grid)
    {
        room_grid = new Room[maze_grid.GetLength(0), maze_grid.GetLength(1)];

        for (int y = 0; y < room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < room_grid.GetLength(0); x++)
            {
                room_grid[x, y] = new Room { size_x = size_x, size_y = size_y, grid_x = (size_x * 3) + 2, grid_y = (size_y * 3) + 2, 
                    num_templates = size_x * size_y};
                room_grid[x, y].offset_x = room_grid[x, y].grid_x * x;
                room_grid[x, y].offset_y = room_grid[x, y].grid_y * y;
                room_grid[x, y].room_object = Instantiate(room_prefab, new Vector3(room_grid[x, y].offset_x, 0, room_grid[x, y].offset_y), Quaternion.identity);
                room_grid[x, y].room_object.transform.parent = transform;
                room_grid[x, y].room_object.GetComponent<GenerateGrid>().StartGenerating(maze_grid[x, y], room_grid[x, y]);
            }
        }
    }
}
