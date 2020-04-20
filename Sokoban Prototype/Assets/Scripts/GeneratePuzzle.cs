using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using enums;
public class GeneratePuzzle : MonoBehaviour
{
    public bool check_solver = true;
    public int min_steps = 8, max_attempts = 10;
    int[,] empty_grid;
    int attempts = 0, running = 0;
    List<Pos> button_positions;
    List<Pos> box_positions;
    public float timer = 0;
    bool timer_started = false, timer_stopped = false;
    public void Generate(Room room)
    {
        if (room.first)
        {
            room.grid[room.entrance.x, room.entrance.y] += (int)Elements.player;
        }
        timer_started = true;
        attempts = 0;
        empty_grid = room.grid.Clone() as int[,];
        StartCoroutine(PlaceButtons(room));
    }

    private void Update()
    {
        if (timer_started && !timer_stopped)
        {
            timer += Time.deltaTime;
        }
    }

    private IEnumerator PlaceButtons(Room room)
    {
        room.grid = empty_grid.Clone() as int[,];
        if (attempts > max_attempts)
        {
            NewRoom(room);
        }
        button_positions = new List<Pos>();
        // Place buttons in valid floor tile positions
        while (button_positions.Count < room.num_boxes)
        {
            int x_pos = Random.Range(1, room.grid.GetLength(0) - 1);
            int y_pos = Random.Range(1, room.grid.GetLength(1) - 1);
            if (room.grid[x_pos, y_pos] == (int)Elements.floor)
            {
                room.grid[x_pos, y_pos] += (int)Elements.button;
                button_positions.Add(new Pos { x = x_pos, y = y_pos });
            }
            yield return null;
        }
        GetDeadCells(room, button_positions);
        attempts++;
    }

    void GetDeadCells(Room room, List<Pos> buttons)
    {
        List<Pos> corners = new List<Pos>();
        // If tile is floor and is in a wall corner, it is marked as a dead square
        for (int y = room.grid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < room.grid.GetLength(0); x++)
            {
                if (room.grid[x, y] == (int)Elements.floor && IsCorner(new Pos(x,y), room))
                {
                    corners.Add(new Pos { x = x, y = y });
                    room.grid[x, y] = (int)Elements.dead;
                }
            }
        }

        // Checks the space between all parralel corner tiles
        // If all of the tiles between the corners are next to a wall
        // All of the tiles between the corners are marked as dead squares
        for (int i = 0; i < corners.Count; i++)
        {
            if (corners.Count > 1)
            {
                for (int j = 1; j < corners.Count; j++)
                {
                    if (corners[i].x == corners[j].x ||
                        corners[i].y == corners[j].y)
                    {
                        bool fill = true, checking = true;
                        List<Pos> spaces = new List<Pos>();
                        Vector2 dir = new Vector2(corners[i].x - corners[j].x, corners[i].y - corners[j].y).normalized;
                        Pos pos = new Pos { x = corners[i].x, y = corners[i].y };

                        while (checking)
                        {
                            pos.x -= (int)dir.x;
                            pos.y -= (int)dir.y;
                            if (pos.x == corners[j].x && pos.y == corners[j].y)
                            {
                                break;
                            }
                            if (!CheckWall(pos, room) ||
                                room.grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.button ||
                                room.grid[pos.x, pos.y] == (int)Elements.wall)
                            {
                                fill = false;
                                break;
                            }

                            spaces.Add(new Pos { x = pos.x, y = pos.y });
                        }


                        if (fill)
                        {
                            string row = "Row: ";
                            for (int k = 0; k < spaces.Count; k++)
                            {
                                row += " x:" + spaces[k].x.ToString() + " y:" + spaces[k].y.ToString() + " |";
                                room.grid[spaces[k].x, spaces[k].y] = (int)Elements.dead;
                            }
                        }
                    }
                }
            }
            corners.Remove(corners[i]);
        }

        PlaceBoxes(room, buttons);
    }

    bool CheckWall(Pos pos, Room room)
    {
        if (room.grid[pos.x, pos.y + 1] == (int)Elements.wall ||
            room.grid[pos.x + 1, pos.y] == (int)Elements.wall ||
            room.grid[pos.x, pos.y - 1] == (int)Elements.wall ||
            room.grid[pos.x - 1, pos.y] == (int)Elements.wall)
        {
            return true;
        }
        return false;
    }
    bool IsCorner(Pos pos, Room room)
    {
        bool[] blocked = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            Pos new_pos = null;
            if (i == 0)
            {
                new_pos = new Pos(pos.x, pos.y + 1);
            }
            if (i == 1)
            {
                new_pos = new Pos(pos.x + 1, pos.y);
            }
            if (i == 2)
            {
                new_pos = new Pos(pos.x, pos.y - 1);
            }
            if (i == 3)
            {
                new_pos = new Pos(pos.x - 1, pos.y);
            }
            if (room.grid[new_pos.x, new_pos.y] == (int)Elements.wall ||
                room.grid[new_pos.x, new_pos.y] == (int)Elements.entrance ||
                room.grid[new_pos.x, new_pos.y] == (int)Elements.exit)
            {
                blocked[i] = true;
            }
        }
        if ((blocked[0] && blocked[1]) || (blocked[1] && blocked[2]) ||
            (blocked[2] && blocked[3]) || (blocked[3] && blocked[0]))
        {
            return true;
        }
        return false;
    }

    void PlaceBoxes(Room room, List<Pos> buttons)
    {
        for (int i = 0; i < room.num_boxes; i++)
        {
            Node box = PlaceBox(new Node { pos = new Pos(buttons[i].x, buttons[i].y) }, room);
            if (box != null && box.depth >= min_steps)
            {
                room.grid[box.pos.x, box.pos.y] += (int)Elements.box;
            }
            else
            {
                StartCoroutine(PlaceButtons(room));
                return;
            }
        }
        if (room.first && room.last)
        {
            if (check_solver)
            {
                StartCoroutine(CheckSolver(room));
            }
            else
            {
                room.generated = true;
                timer_stopped = true;
            }
        }
        else
        {
            StartCoroutine(CheckPath(room));
        }
    }


    class Node
    {
        public Pos pos;
        public int depth = 0;
    }

    Node PlaceBox(Node start_node, Room room)
    {
        List<Node> open_list = new List<Node>();
        HashSet<Node> closed_list = new HashSet<Node>();
        open_list.Add(start_node);

        while (open_list.Count > 0)
        {
            Node current_node = open_list[0];
            open_list.Remove(current_node);
            closed_list.Add(current_node);

            for (int i = 0; i < 4; i ++)
            {
                Pos pos = IsFree((Direction)i, room.grid, current_node.pos);
                if (pos != null)
                {
                    Node new_node = new Node { pos = pos, depth = current_node.depth + 1 };
                    bool contains = false;
                    foreach (Node node in open_list)
                    { 
                        if (node.pos.x == new_node.pos.x &&
                            node.pos.y == new_node.pos.y)
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains)
                    {
                        foreach (Node node in closed_list)
                        {
                            if (node.pos.x == new_node.pos.x &&
                                node.pos.y == new_node.pos.y)
                            {
                                contains = true;
                                break;
                            }
                        }
                    }
                    if (!contains)
                    { 
                        open_list.Add(new_node);
                    }
                }
            }

            open_list = open_list.OrderByDescending(w => w.depth).ToList();
        }

        Node deepest_node = null;
        int node_depth = 0;
        foreach (Node node in closed_list)
        {
            if (node.depth > node_depth)
            {
                node_depth = node.depth;
                deepest_node = node;
            }
        }

        return deepest_node;
    }

    Pos IsFree(Direction dir, int[,] grid, Pos pos)
    {
        if (dir == Direction.N && grid[pos.x, pos.y + 1] == (int)Elements.floor &&
            grid[pos.x, pos.y + 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y + 1 };
        }
        if (dir == Direction.E && grid[pos.x + 1, pos.y] == (int)Elements.floor &&
            grid[pos.x + 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x + 1, y = pos.y };
        }
        if (dir == Direction.S && grid[pos.x, pos.y - 1] == (int)Elements.floor &&
            grid[pos.x, pos.y - 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y - 1 };
        }
        if (dir == Direction.W && grid[pos.x - 1, pos.y] == (int)Elements.floor &&
            grid[pos.x - 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x - 1, y = pos.y };
        }
        return null;
    }

    void NewRoom(Room room)
    {
        StopAllCoroutines();
        GetComponent<GenerateGrid>().Restart(room);
    }

    private Direction RandomDir() { return (Direction)Random.Range(0, 4); }
    private Pos CheckDir(Direction dir, int[,] grid, Pos pos)
    {
        // Checks if tile in direction is empty floor
        if (dir == Direction.N && grid[pos.x, pos.y + 1] == (int)Elements.floor &&
            grid[pos.x, pos.y + 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y + 1 };
        }
        if (dir == Direction.E && grid[pos.x + 1, pos.y] == (int)Elements.floor && 
            grid[pos.x + 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x + 1, y = pos.y };
        }
        if (dir == Direction.S && grid[pos.x, pos.y - 1] == (int)Elements.floor && 
            grid[pos.x, pos.y - 2] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y - 1 };
        }
        if (dir == Direction.W && grid[pos.x - 1, pos.y] == (int)Elements.floor && 
            grid[pos.x - 2, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x - 1, y = pos.y };
        }
        return new Pos { empty = true };
    }

    IEnumerator CheckPath(Room room)
    {
        for (int i = 0; i < room.exits.Count; i++)
        {
            float time_checked = 0;
            FindPath path = new FindPath(room.grid, room.entrance, room.exits[i]);
            bool checking = true;
            while (checking)
            {
                time_checked += Time.deltaTime;
                if (path.final_path.Count > 0)
                {
                    checking = false;
                }
                if (time_checked >= 1)
                {
                    Debug.Log("Path Failed");
                    StartCoroutine(PlaceButtons(room));
                    yield break;
                }
                yield return null;
            }
        }

        if (check_solver)
        {
            StartCoroutine(CheckSolver(room));
        }
        else
        {
            room.generated = true;
            timer_stopped = true;
        }
    }

    IEnumerator CheckSolver(Room room)
    {
        Attempt attempt = new Attempt() { solved = false, failed = false};
        Debug.Log("STARTED SOLVER");
        Solver solver = GetComponent<Solver>();
        solver.StartSolving(room, attempt);
        // Wait until solver has failed or succeeded in solving the puzzle
        while (!attempt.failed && !attempt.solved)
        {
            yield return null;
        }
        if (attempt.failed)
        {
            StartCoroutine(PlaceButtons(room));
            Debug.Log("FAILED");
        }
        if (attempt.solved)
        {
            Debug.Log("SOLVED");
            room.generated = true;
        }
    }
}
