using System.Collections.Generic;
using enums;
public class GridCheck
{
    public int FloorCount(int[,] grid)
    {
        int num_floors = 0;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == (int)Elements.floor || grid[x,y] == (int)Elements.floor + (int)Elements.button)
                {
                    num_floors++;
                }
            }
        }

        // If the number of floors is greater than quater of the number of total tiles, floor check is passed
        return num_floors;
    }

    public bool ContinuousFloor(int[,] grid, int num_floors)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == (int)Elements.floor)
                {
                    return CheckContinuous(grid, new Pos(x, y), num_floors);
                }
            }
        }
        return false;
    }

    public bool CheckContinuous(int[,] grid, Pos start, int num_floors)
    {
        List<Pos> open_list = new List<Pos>();
        List<Pos> closed_list = new List<Pos>();
        open_list.Add(start);

        // Loop until checks returns to start position
        while (open_list.Count > 0)
        {
            Pos current = open_list[0];
            open_list.Remove(current);
            closed_list.Add(current);
            for (int i = 0; i < 4; i++)
            {
                Pos position = current.GetNewPos((Direction)i);
                bool contains = false;
                foreach (Pos pos in open_list)
                {
                    if (pos.x == position.x &&
                        pos.y == position.y)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                {
                    foreach (Pos pos in closed_list)
                    {
                        if (pos.x == position.x &&
                            pos.y == position.y)
                        {
                            contains = true;
                            break;
                        }
                    }
                }
                if (!contains && grid[position.x, position.y] == (int)Elements.floor)
                {
                    open_list.Add(position);
                }
            }
        }

        return (closed_list.Count == num_floors);
    }

    //public bool FillGaps()
    //{
    //    // If a floor tile is surrounded by wall tiles (in 3 or more directions) fill in with wall tile
    //    int max_passes = 8;
    //    for (int i = 0; i < max_passes; i++)
    //    {
    //        int highest_walls = 0;
    //        for (int y = 0; y < grid.GetLength(1); y++)
    //        {
    //            for (int x = 0; x < grid.GetLength(0); x++)
    //            {
    //                int surrounding_walls = 0;
    //                if (grid[x, y] == 1)
    //                {
    //                    if (grid[x + 1, y] == 2) surrounding_walls++;
    //                    if (grid[x - 1, y] == 2) surrounding_walls++;
    //                    if (grid[x, y + 1] == 2) surrounding_walls++;
    //                    if (grid[x, y - 1] == 2) surrounding_walls++;

    //                    if (surrounding_walls > highest_walls) highest_walls = surrounding_walls;

    //                    if (surrounding_walls >= 3)
    //                    {
    //                        grid[x, y] = 2;
    //                    }
    //                }
    //            }
    //        }

    //        if (highest_walls < 3)
    //        {
    //            break;
    //        }
    //    }
    //    return true;
    //}
}
