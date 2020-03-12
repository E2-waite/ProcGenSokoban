﻿using System.Collections.Generic;
using enums;
public class GridCheck
{
    public int num_floors = 0;
    public int checks_left = 2;
    int[] first_floor;
    int[,] grid;

    public GridCheck(int[,] check_grid)
    {
        grid = check_grid;
    }
    public GridCheck()
    {

    }
    public bool FloorCount()
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == 1)
                {
                    if (num_floors == 0)
                    {
                        first_floor = new int[2] { x, y };
                    }
                    num_floors++;
                }
            }
        }

        // If the number of floors is greater than quater of the number of total tiles, floor check is passed
        return (num_floors > (grid.GetLength(0) * grid.GetLength(1)) / 4);
    }

    public bool ContinuousFloor(int num_floors)
    {
        List<int[]> checked_floor = new List<int[]>();
        Direction dir = Direction.E;
        int num_checked = 0;
        checked_floor.Add(first_floor);

        // Loop until checks returns to start position
        while (checked_floor.Count > 0)
        {
            bool placed = false;
            for (int i = 0; i < 4; i++)
            {
                int[] pos = CheckDir(checked_floor[checked_floor.Count - 1], dir);
                // If direction is free, add to checked list and set grid position to 0
                if (pos[0] != 0 && pos[1] != 0)
                {
                    grid[pos[0], pos[1]] = 0;
                    checked_floor.Add(pos);
                    num_checked++;
                    placed = true;
                    break;
                }

                if (dir == Direction.W) dir = Direction.N;
                else dir++;
            }

            if (!placed)
            {
                // If no surrounding tiles are floor remove last position in checked floor list (backtracks 1 space)
                checked_floor.Remove(checked_floor[checked_floor.Count - 1]);
            }
        }

        // Need to set checked spaces back to 1 (floor), NEED TO MAKE THE GRID PASSED BY VALUE NOT PASSED BY REFERENCE
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == 0)
                {
                    grid[x, y] = 1;
                }
            }
        }
        return (num_checked == num_floors);
    }

    private int[] CheckDir(int[] pos, Direction dir)
    {
        if (dir == Direction.N && grid[pos[0], pos[1] + 1] == 1) return new int[2] { pos[0], pos[1] + 1 };
        if (dir == Direction.E && grid[pos[0] + 1, pos[1]] == 1) return new int[2] { pos[0] + 1, pos[1] };
        if (dir == Direction.S && grid[pos[0], pos[1] - 1] == 1) return new int[2] { pos[0], pos[1] - 1 };
        if (dir == Direction.W && grid[pos[0] - 1, pos[1]] == 1) return new int[2] { pos[0] - 1, pos[1] };
        return new int[2] { 0, 0 };
    }

    public int[,] FillGaps()
    {
        // If a floor tile is surrounded by wall tiles (in 3 or more directions) fill in with wall tile
        int max_passes = 8;
        for (int i = 0; i < max_passes; i++)
        {
            int highest_walls = 0;
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    int surrounding_walls = 0;
                    if (grid[x, y] == 1)
                    {
                        if (grid[x + 1, y] == 2) surrounding_walls++;
                        if (grid[x - 1, y] == 2) surrounding_walls++;
                        if (grid[x, y + 1] == 2) surrounding_walls++;
                        if (grid[x, y - 1] == 2) surrounding_walls++;

                        if (surrounding_walls > highest_walls) highest_walls = surrounding_walls;

                        if (surrounding_walls >= 3)
                        {
                            grid[x, y] = 2;
                        }
                    }
                }
            }

            if (highest_walls < 3)
            {
                break;
            }
        }
        return grid;
    }
}
