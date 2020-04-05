using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateLevel : MonoBehaviour
{
    public Level Generate(int size_x, int size_y, int grid_x, int grid_y, int maze_x, int maze_y)
    {
        Level level = new Level();
        StartCoroutine(GenerationRoutine(level, size_x, size_y, grid_x, grid_y, maze_x, maze_y));
        return level;
    }

    public IEnumerator GenerationRoutine(Level level, int size_x, int size_y, int grid_x, int grid_y, int maze_x, int maze_y)
    {
        Maze maze = GetComponent<GenerateMaze>().Generate(maze_x, maze_y);
        while (!maze.complete)
        {
            yield return null;
        }

        level.room_grid = new Room[maze.grid.GetLength(0), maze.grid.GetLength(1)];

        // Finds deepest maze node, to choose where the transition to next level is placed
        Cell shallowest_cell = null;
        Cell deepest_cell = null;
        int lowest_depth = 5;
        int highest_depth = 0;
        for (int y = 0; y < maze.grid.GetLength(1); y++)
        {
            for (int x = 0; x < maze.grid.GetLength(0); x++)
            {
                if (maze.grid[x,y].depth > highest_depth)
                {
                    deepest_cell = maze.grid[x, y];
                    highest_depth = maze.grid[x, y].depth;
                }
                if (maze.grid[x,y].depth < lowest_depth)
                {
                    shallowest_cell = maze.grid[x, y];
                    lowest_depth = maze.grid[x, y].depth;
                }
            }
        }
        shallowest_cell.first_room = true;
        deepest_cell.last_room = true;

        // Create and populate room grid
        for (int y = 0; y < level.room_grid.GetLength(1); y++)
        {
            for (int x = 0; x < level.room_grid.GetLength(0); x++)
            {
                level.room_grid[x, y] = new Room { size_x = size_x, size_y = size_y, grid_x = grid_x, grid_y = grid_y,
                    num_templates = size_x * size_y };
                level.room_grid[x, y].offset_x = level.room_grid[x, y].grid_x * x;
                level.room_grid[x, y].offset_y = level.room_grid[x, y].grid_y * y;
                level.room_grid[x, y].first = maze.grid[x, y].first_room;
                level.room_grid[x, y].last = maze.grid[x, y].last_room;
                level.room_grid[x, y].pos = new Pos { x = x, y = y };
                GetComponent<GenerateGrid>().StartGenerating(maze.grid[x, y], level.room_grid[x, y]);
                while (!level.room_grid[x, y].generated)
                {
                    yield return null;
                }
            }
        }

        level.generated = true;
    }
}
