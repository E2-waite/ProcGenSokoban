using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateLevel : MonoBehaviour
{
    public int size_x = 3;
    int size_y = 3, grid_x, grid_y;
    public GameObject room_prefab;
    Room[,] room_grid;
    public int[,] grid;
    public GameObject[,] object_grid;

    public void Generate(Cell[,] maze_grid)
    {
        StartCoroutine(GenerationRoutine(maze_grid));
    }

    public IEnumerator GenerationRoutine(Cell[,] maze_grid)
    {
        grid_x = (size_x * 3) + 2;
        grid_y = (size_y * 3) + 2;
        room_grid = new Room[maze_grid.GetLength(0), maze_grid.GetLength(1)];

        for (int y = 0; y < room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < room_grid.GetLength(0); x++)
            { 
                room_grid[x, y] = new Room { size_x = size_x, size_y = size_y, grid_x = grid_x, grid_y = grid_y, 
                    num_templates = size_x * size_y};
                room_grid[x, y].offset_x = room_grid[x, y].grid_x * x;
                room_grid[x, y].offset_y = room_grid[x, y].grid_y * y;
                room_grid[x, y].room_object = Instantiate(room_prefab, new Vector3(room_grid[x, y].offset_x, 0, room_grid[x, y].offset_y), Quaternion.identity);
                room_grid[x, y].room_object.transform.parent = transform;
                room_grid[x, y].first = maze_grid[x, y].first_room;
                room_grid[x, y].room_object.GetComponent<GenerateGrid>().StartGenerating(maze_grid[x, y], room_grid[x, y]);
            }
        }

        bool all_generated = false;
        while (!all_generated)
        {
            int num_generated = 0;
            for (int y = 0; y < room_grid.GetLength(1); y++)
            {
                for (int x = 0; x < room_grid.GetLength(0); x++)
                {
                    if (room_grid[x,y].generated)
                    {
                        num_generated++;
                    }
                    yield return null;
                }
            }

            if (num_generated == room_grid.GetLength(0) * room_grid.GetLength(1))
            {
                all_generated = true;
            }
        }

        grid = new int[room_grid.GetLength(0) * grid_x, room_grid.GetLength(1) * grid_y];
        object_grid = new GameObject[room_grid.GetLength(0) * grid_x, room_grid.GetLength(1) * grid_y];
        for (int y = 0; y < room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < room_grid.GetLength(0); x++)
            {
                for (int iy = 0; iy < grid_y; iy++)
                {
                    for (int ix = 0; ix < grid_x; ix++)
                    {
                        grid[ix + (x * grid_x), iy + (y * grid_y)] = room_grid[x, y].grid[ix, iy];
                        object_grid[ix + (x * grid_x), iy + (y * grid_y)] = room_grid[x, y].object_grid[ix, iy];
                    }
                }
            }
        }

        GetComponent<GameControl>().StartGame(object_grid);
        PlacePlayer();
    }

    void PlacePlayer()
    {
        
    }
}
