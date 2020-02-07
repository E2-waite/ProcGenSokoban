using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateMaze : MonoBehaviour
{
    public GameObject tile_prefab;
    public Vector2 size;
    private Cell[,] grid;
    readonly private List<Cell> checked_cells = new List<Cell>();
    public int junctions = 0, dead_ends = 0;
    void Start()
    {
        grid = new Cell[(int)size.x, (int)size.y];
        StartCoroutine(LoopCheck());
        StartCoroutine(StepForward(0, 2, Direction.E));
    }

    IEnumerator LoopCheck()
    {
        // Starts loop that checks if full grid is filled
        if (CheckCompletion())
        {
            StopAllCoroutines();
            dead_ends++;
        }
        yield return new WaitForSeconds(0.001f);
        StartCoroutine(LoopCheck());
    }

    IEnumerator StepForward(int x, int y, Direction dir)
    {
        // Adds grid tile to checked tiles list, then check adjascent tiles for clear space
        grid[x, y] = new Cell(x,y, dir);
        checked_cells.Add(grid[x, y]);
        dir = RandomDir();
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = GetNewPos(dir, x, y);
            if (InGrid((int)pos.x, (int)pos.y) && IsEmpty((int)pos.x, (int)pos.y))
            {
                // If space is clear, continue to next step
                grid[x, y].ClearWall(dir);
                StartCoroutine(StepForward((int)pos.x, (int)pos.y, dir));
                yield break;
            }
            if (dir == Direction.W) dir = Direction.N;
            else dir++;
            yield return null;
        }
        // Start backtracking if there are no clear space adjascent to current tile (Dead end)
        dead_ends++;
        StartCoroutine(StepBack());
    }

    IEnumerator StepBack()
    {
        // Backtrack through checked cells list
        checked_cells.Remove(checked_cells[checked_cells.Count - 1]);
        Cell curr_cell = checked_cells[checked_cells.Count - 1];

        int x = curr_cell.GetX(), y = curr_cell.GetY();
        Direction dir = RandomDir();
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = GetNewPos(dir, x, y);
            if (InGrid((int)pos.x, (int)pos.y) && IsEmpty((int)pos.x, (int)pos.y))
            {
                // Check adjacent tiles to cell at top of checked list, move in new direction if space is clear (Junction)
                grid[x, y].ClearWall(dir);
                junctions++;
                StartCoroutine(StepForward((int)pos.x, (int)pos.y, dir));
                yield break;
            }
            if (dir == Direction.W) dir = Direction.N;
            else dir++;
            yield return null;
        }
        StartCoroutine(StepBack());
    }

    Vector2 GetNewPos(Direction dir, int x, int y)
    {
        if (dir == Direction.N) return new Vector2(x, y + 1);
        else if (dir == Direction.E) return new Vector2(x + 1, y);
        else if (dir == Direction.S) return new Vector2(x, y - 1);
        else if (dir == Direction.W) return new Vector2(x - 1, y);
        return new Vector2(0, 0);
    }

    bool IsEmpty(int x, int y)
    {
        if (grid[x, y] == null) return true;
        else return false;
    }

    bool InGrid(int x, int y)
    {
        if (x < 0 || x >= (int)size.x || y < 0 || y >= (int)size.y) return false;
        else return true;
    }

    bool CheckCompletion()
    {
        for (int x = 0; x < (int)size.x; x++)
        {
            for (int y = 0; y < (int)size.y; y++)
            {
                if (grid[x, y] == null)
                {
                    return false;
                }
            }
        }
        Debug.Log("FILLED");
        InstantiateMaze();
        return true;
    }

    Direction RandomDir()
    {
        return (Direction)Random.Range(0, 4);
    }

    void InstantiateMaze()
    {
        for (int x = 0; x < (int)size.x; x++)
        {
            for (int y = 0; y < (int)size.y; y++)
            {
                GameObject tile = Instantiate(tile_prefab, new Vector3(x, 0, y), Quaternion.identity);
                tile.GetComponent<RenderWall>().EnableWalls(grid[x, y].GetWalls());
            }
        }
    }
}
