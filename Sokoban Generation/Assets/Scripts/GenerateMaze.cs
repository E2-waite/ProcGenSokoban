using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using enums;
public class GenerateMaze : MonoBehaviour
{
    int max_depth, steps_back, max_rooms;
    public List<Cell> GetMaze(Cell[,] grid, int depth, int steps, int rooms)
    {
        max_depth = depth;
        steps_back = steps;
        max_rooms = rooms;
        IntVec2 start_pos = new IntVec2(Mathf.RoundToInt(grid.GetLength(0) / 2), Mathf.RoundToInt(grid.GetLength(1) / 2));
        List<Cell> cells = new List<Cell>();
        Cell first_cell = new Cell(start_pos.x, start_pos.y, Direction.None) { first_room = true };
        grid[first_cell.pos.x, first_cell.pos.y] = first_cell;
        cells.Add(first_cell);
        return Step(cells, grid, first_cell);
    }

    List<Cell> Step (List<Cell> cells, Cell[,] grid, Cell current)
    {
        if (cells.Count < max_rooms)
        {
            // If any cells are not filled, continue else return list of cells
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y] == null)
                    {
                        goto step;
                    }
                }
            }
        }
        cells = cells.OrderBy(w => w.depth).ToList();
        cells[cells.Count - 1].last_room = true;
        return cells;

    step:

        Direction dir = RandomDir();
        if (current.depth <= max_depth)
        {
            for (int i = 0; i < 4; i++)
            {
                Pos pos = current.pos.GetNewPos(dir);
                if (InGrid(pos, grid) && grid[pos.x, pos.y] == null)
                {
                    grid[pos.x, pos.y] = new Cell(pos.x, pos.y, dir, current) { depth = current.depth + 1};
                    cells.Add(grid[pos.x, pos.y]);
                    current.exits.Add(dir);
                    return Step(cells, grid, grid[pos.x, pos.y]);
                }
                if (dir == Direction.W)
                {
                    dir = Direction.N;
                }
                else
                {
                    dir++;
                }
            }
        }
        if (current.parent == null)
        {
            cells = cells.OrderBy(w => w.depth).ToList();
            cells[cells.Count - 1].last_room = true;
            return cells;
        }
        else
        {
            Cell parent_cell = current;
            for (int i = 0; i < steps_back; i++)
            {
                if (parent_cell.parent != null)
                {
                    parent_cell = parent_cell.parent;
                }
            }
            return Step(cells, grid, parent_cell);
        }
    }


    bool InGrid(Pos pos, Cell[,] grid)
    {
        if (pos.x < 0 || pos.x >= grid.GetLength(0) || pos.y < 0 || pos.y >= grid.GetLength(1)) return false;
        else return true;
    }

    Direction RandomDir()
    {
        //Random.InitState((int)Time.time);
        return (Direction)Random.Range(0, 4);
    }
}
