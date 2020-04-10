using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        
        // Gets maze cells and puts them in order of depth
        level.maze_cells = new List<Cell>();
        for (int y = 0; y < maze.grid.GetLength(1); y++)
        {
            for (int x = 0; x < maze.grid.GetLength(0); x++)
            {
                level.maze_cells.Add(maze.grid[x, y]);
            }
        }

        level.maze_cells = level.maze_cells.OrderBy(w => w.depth).ToList();
        level.maze_cells[0].first_room = true;
        level.maze_cells[level.maze_cells.Count - 1].last_room = true;

        // Generates the room layouts of the room grid
        for (int i = 0; i < level.maze_cells.Count; i++)
        {
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y] = new Room { size_x = size_x, size_y = size_y, grid_x = grid_x, grid_y = grid_y,
                    num_templates = size_x * size_y };
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].parent_level = level;
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].offset_x = 
                level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].grid_x * level.maze_cells[i].GetPos().x;
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].offset_y = 
                level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].grid_y * level.maze_cells[i].GetPos().y;
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].first = 
                maze.grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].first_room;
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].last =
                maze.grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].last_room;
            level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].pos = 
                new Pos { x = level.maze_cells[i].GetPos().x, y = level.maze_cells[i].GetPos().y };
            GetComponent<GenerateGrid>().StartGenerating(maze.grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y], 
                level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y]);
            while (!level.room_grid[level.maze_cells[i].GetPos().x, level.maze_cells[i].GetPos().y].generated)
            {
                yield return null;
            }
        }

        level.generated = true;
    }
}
