using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateMaze : MonoBehaviour
{
    Vector2 size;
    readonly private List<Cell> checked_cells = new List<Cell>();
    public int junctions = 0, dead_ends = 0;
    int depth = 0;

    public Maze Generate(int maze_x, int maze_y)
    {
        Maze maze = new Maze();
        size = new Vector2(maze_x, maze_y);
        maze.grid = new Cell[(int)size.x, (int)size.y];
        StartCoroutine(LoopCheck(maze));
        StartCoroutine(StepForward(0, 0, Direction.E, maze));
        return maze;
    }

    IEnumerator LoopCheck(Maze maze)
    {
        // Starts loop that checks if full grid is filled
        if (CheckCompletion(maze))
        {
            StopAllCoroutines();
            dead_ends++;
        }
        yield return new WaitForSeconds(0.001f);
        StartCoroutine(LoopCheck(maze));
    }

    IEnumerator StepForward(int x, int y, Direction dir, Maze maze)
    {
        depth++;
        // Adds grid tile to checked tiles list, then check adjascent tiles for clear space
        maze.grid[x, y] = new Cell(x,y, dir);
        maze.grid[x, y].depth = depth;
        checked_cells.Add(maze.grid[x, y]);

        dir = RandomDir();
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = GetNewPos(dir, x, y);
            if (InGrid((int)pos.x, (int)pos.y) && IsEmpty((int)pos.x, (int)pos.y, maze))
            {
                // If space is clear, continue to next step
                maze.grid[x, y].exits.Add(dir);
                StartCoroutine(StepForward((int)pos.x, (int)pos.y, dir, maze));
                yield break;
            }
            if (dir == Direction.W) dir = Direction.N;
            else dir++;
            yield return null;
        }
        // Start backtracking if there are no clear space adjascent to current tile (Dead end)
        dead_ends++;
        StartCoroutine(StepBack(maze));
    }

    IEnumerator StepBack(Maze maze)
    {
        depth--;
        // Backtrack through checked cells list
        checked_cells.Remove(checked_cells[checked_cells.Count - 1]);
        Cell curr_cell = checked_cells[checked_cells.Count - 1];

        int x = curr_cell.GetPos().x, y = curr_cell.GetPos().y;
        Direction dir = RandomDir();
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = GetNewPos(dir, x, y);
            if (InGrid((int)pos.x, (int)pos.y) && IsEmpty((int)pos.x, (int)pos.y, maze))
            {
                // Check adjacent tiles to cell at top of checked list, move in new direction if space is clear (Junction)
                junctions++;
                maze.grid[x, y].exits.Add(dir);
                StartCoroutine(StepForward((int)pos.x, (int)pos.y, dir, maze));
                yield break;
            }
            if (dir == Direction.W) dir = Direction.N;
            else dir++;
            yield return null;
        }
        StartCoroutine(StepBack(maze));
    }

    Vector2 GetNewPos(Direction dir, int x, int y)
    {
        if (dir == Direction.N) return new Vector2(x, y + 1);
        else if (dir == Direction.E) return new Vector2(x + 1, y);
        else if (dir == Direction.S) return new Vector2(x, y - 1);
        else if (dir == Direction.W) return new Vector2(x - 1, y);
        return new Vector2(0, 0);
    }

    bool IsEmpty(int x, int y, Maze maze)
    {
        if (maze.grid[x, y] == null) return true;
        else return false;
    }

    bool InGrid(int x, int y)
    {
        if (x < 0 || x >= (int)size.x || y < 0 || y >= (int)size.y) return false;
        else return true;
    }

    bool CheckCompletion(Maze maze)
    {
        for (int x = 0; x < (int)size.x; x++)
        {
            for (int y = 0; y < (int)size.y; y++)
            {
                if (maze.grid[x, y] == null)
                {
                    return false;
                }
            }
        }
        Debug.Log("FILLED");

        maze.complete = true;
        return true;
    }

    Direction RandomDir()
    {
        return (Direction)Random.Range(0, 4);
    }
}
