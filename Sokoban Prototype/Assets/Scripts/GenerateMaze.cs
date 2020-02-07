using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateMaze : MonoBehaviour
{
    public GameObject tile_prefab;
    public int x_size, y_size;
    public bool maze_complete = false;
    private Cell[,] grid;
    readonly private List<Cell> checked_cells = new List<Cell>();

    void Start()
    {
        grid = new Cell[x_size, y_size];
        StartCoroutine(LoopCheck());
        StartCoroutine(StepForward(0, 0, Direction.N));
    }

    IEnumerator LoopCheck()
    {
        // Starts loop that checks if full grid is filled
        if (CheckCompletion()) StopAllCoroutines();
        yield return new WaitForSeconds(0.01f);
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
        // Start backtracking if there are no clear space adjascent to current tile
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
                // If the cell at the top of the checked cells list has clear space adjascent, make junction and start branching out in that direction
                grid[x, y].ClearWall(dir);
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
        if (x < 0 || x >= x_size || y < 0 || y >= y_size) return false;
        else return true;
    }

    bool CheckCompletion()
    {
        for (int x = 0; x < x_size; x++)
        {
            for (int y = 0; y < y_size; y++)
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
        for (int x = 0; x < x_size; x++)
        {
            for (int y = 0; y < y_size; y++)
            {
                GameObject tile = Instantiate(tile_prefab, new Vector3(x, 0, y), Quaternion.identity);
                tile.GetComponent<RenderWall>().EnableWalls(grid[x, y].GetWalls());
            }
        }
    }

}
