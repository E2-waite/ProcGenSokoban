using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using enums;
public class GenerateMaze
{
    public List<Cell> Generate(Cell[,] grid)
    {
        List<Cell> maze_list = new List<Cell>();
        List<Cell> stack = new List<Cell>();
        IntVec2 start_pos = new IntVec2(Mathf.RoundToInt(grid.GetLength(0) / 2), Mathf.RoundToInt(grid.GetLength(1) / 2));
        grid[start_pos.x, start_pos.y] = new Cell(start_pos.x, start_pos.y, Direction.None);
        stack.Add(new Cell(start_pos.x, start_pos.y, Direction.None));
        while (stack.Count > 0)
        {
            Cell current_cell = stack[0];
            stack.Remove(current_cell);

            if (grid[current_cell.pos.x, current_cell.pos.y] != null)
            {
                continue;
            }

            maze_list.Add(current_cell);
            grid[current_cell.pos.x, current_cell.pos.x] = current_cell;
            Direction dir = RandomDir();

            List<Cell> surrounding_cells = new List<Cell>();
            for (int i = 0; i < 4; i++)
            {
                Pos new_pos = GetNewPos(dir, current_cell.pos);
                if (InGrid(new_pos, grid))
                {
                    surrounding_cells.Add(new Cell(new_pos.x, new_pos.y, dir, current_cell));
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

            foreach (Cell cell in surrounding_cells)
            {
                if (grid[cell.pos.x, cell.pos.y] != null)
                {
                    stack.Insert(0, cell);
                }
            }
        }
        maze_list = maze_list.OrderBy(w => w.depth).ToList();
        foreach (Cell cell in maze_list)
        {
            cell.AddParentExit();
        }
        return maze_list;
    }

    Pos GetNewPos(Direction dir, Pos pos)
    {
        if (dir == Direction.N) return new Pos(pos.x, pos.y + 1);
        else if (dir == Direction.E) return new Pos(pos.x + 1, pos.y);
        else if (dir == Direction.S) return new Pos(pos.x, pos.y - 1);
        else if (dir == Direction.W) return new Pos(pos.x - 1, pos.y);
        return null;
    }

    bool InGrid(Pos pos, Cell[,] grid)
    {
        if (pos.x < 0 || pos.x >= grid.GetLength(0) || pos.y < 0 || pos.y >= grid.GetLength(1)) return false;
        else return true;
    }

    Direction RandomDir()
    {
        return (Direction)Random.Range(0, 4);
    }
}
