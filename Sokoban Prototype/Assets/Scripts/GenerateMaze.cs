using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using enums;
public class GenerateMaze : MonoBehaviour
{
    public List<Cell> GetMaze(Cell[,] grid)
    {
        IntVec2 start_pos = new IntVec2(Mathf.RoundToInt(grid.GetLength(0) / 2), Mathf.RoundToInt(grid.GetLength(1) / 2));
        List<Cell> cells = new List<Cell>();
        Cell first_cell = new Cell(start_pos.x, start_pos.y, Direction.None) { first_room = true };
        cells.Add(first_cell);
        return Step(cells, grid);
    }

    List<Cell> Step (List<Cell> cells, Cell[,] grid)
    {
        // If any cells are not filled, continue else return list of cells
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x,y] == null)
                {
                    goto step;
                }
            }
        }
        return cells;

    step:
        bool placed = false;
        Cell current_cell = cells[cells.Count - 1];
        while (!placed)
        {
            Direction dir = RandomDir();
            for (int i = 0; i < 4; i++)
            {
                Pos pos = GetNewPos((Direction)i, current_cell.pos);
                if (InGrid(pos, grid) && grid[pos.x, pos.y] == null)
                {
                    grid[pos.x, pos.y] = new Cell(pos.x, pos.y, dir, current_cell);
                    cells.Add(grid[pos.x, pos.y]);
                    return Step(cells, grid);
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
            current_cell = current_cell.parent;
        }
        return null;
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
