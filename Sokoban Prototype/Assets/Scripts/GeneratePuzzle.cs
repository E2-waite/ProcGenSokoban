using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using enums;
public class GeneratePuzzle
{
    public bool check_solver = true;
    public int min_steps = 8, max_attempts = 10;
    int[,] empty_grid;
    int attempts = 0;
    public bool Generate(Room room)
    {
        if (room.first)
        {
            room.grid[room.entrance.x, room.entrance.y] += (int)Elements.player;
        }
        attempts = 0;
        empty_grid = room.grid.Clone() as int[,];
        return PlaceButtons(room);
    }

    private bool PlaceButtons(Room room)
    {
        for (int i = 0; i < max_attempts; i++)
        {
            room.grid = empty_grid.Clone() as int[,];
            List<Pos> buttons = new List<Pos>();
            // Place buttons in valid floor tile positions
            while (buttons.Count < room.num_boxes)
            {
                int x_pos = Random.Range(1, room.grid.GetLength(0) - 1);
                int y_pos = Random.Range(1, room.grid.GetLength(1) - 1);
                if (room.grid[x_pos, y_pos] == (int)Elements.floor)
                {
                    room.grid[x_pos, y_pos] += (int)Elements.button;
                    buttons.Add(new Pos { x = x_pos, y = y_pos });
                }
            }
            if (GetDeadCells(room, buttons))
            {
                room.generated = true;
                return true;
            }
        }
        return false;
    }

    bool GetDeadCells(Room room, List<Pos> buttons)
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
                            for (int k = 0; k < spaces.Count; k++)
                            {
                                room.grid[spaces[k].x, spaces[k].y] = (int)Elements.dead;
                            }
                        }
                    }
                }
            }
            corners.Remove(corners[i]);
        }

        return PlaceBoxes(room, buttons);
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
                new_pos = new Pos { x = pos.x, y = pos.y + 1 };
            }
            if (i == 1)
            {
                new_pos = new Pos { x = pos.x + 1, y = pos.y };
            }
            if (i == 2)
            {
                new_pos = new Pos { x = pos.x, y = pos.y - 1 };
            }
            if (i == 3)
            {
                new_pos = new Pos { x = pos.x - 1, y = pos.y };
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

    bool PlaceBoxes(Room room, List<Pos> buttons)
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
                return false;
            }
        }

        if (room.first && room.last)
        {
            return true;
        }
        else
        {
            return CheckPath(room);
        }
    }


    class Node
    {
        public Pos pos;
        public Pos player_pos;
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
                Pos box_pos = BoxPos((Direction)i, room.grid, current_node.pos);
                Pos push_pos = PushPos((Direction)i, room.grid, current_node.pos);
                if (box_pos != null && push_pos != null)
                {
                    FindPath path = new FindPath(room.grid, Elements.box);
                    if (current_node.player_pos == null || path.IsPath(current_node.player_pos, push_pos))
                    {
                        Node new_node = new Node { pos = box_pos, depth = current_node.depth + 1, player_pos = push_pos };
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

    Pos BoxPos(Direction dir, int[,] grid, Pos pos)
    {
        if (dir == Direction.N && grid[pos.x, pos.y + 1] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y + 1 };
        }
        if (dir == Direction.E && grid[pos.x + 1, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x + 1, y = pos.y };
        }
        if (dir == Direction.S && grid[pos.x, pos.y - 1] == (int)Elements.floor)
        {
            return new Pos { x = pos.x, y = pos.y - 1 };
        }
        if (dir == Direction.W && grid[pos.x - 1, pos.y] == (int)Elements.floor)
        {
            return new Pos { x = pos.x - 1, y = pos.y };
        }
        return null;
    }

    Pos PushPos(Direction dir, int[,] grid, Pos pos)
    {
        Pos new_pos = null;
        if (dir == Direction.N)
        {
            new_pos = new Pos { x = pos.x, y = pos.y + 2 };
        }
        if (dir == Direction.E)
        {
            new_pos = new Pos { x = pos.x + 2, y = pos.y };
        }
        if (dir == Direction.S)
        {
            new_pos = new Pos { x = pos.x, y = pos.y - 2 };
        }
        if (dir == Direction.W)
        {
            new_pos = new Pos { x = pos.x - 2, y = pos.y };
        }
        if (new_pos.x >= 0 && new_pos.x < grid.GetLength(0) && new_pos.y >= 0 && new_pos.y < grid.GetLength(1) &&
            grid[new_pos.x, new_pos.y] == (int)Elements.floor)
        {
            return new_pos;
        }
        return null;
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

    bool CheckPath(Room room)
    {
        for (int i = 0; i < room.exits.Count; i++)
        {
            FindPath path = new FindPath(room.grid, Elements.button);
            if (!path.IsPath(room.entrance, room.exits[i]))
            {
                return false;
            }
        }
        return true;
    }
}
