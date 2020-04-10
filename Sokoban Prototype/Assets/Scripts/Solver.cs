using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Solver : MonoBehaviour
{
    Pos[] button_pos;

    class Node
    {
        public List<Node> parents = new List<Node>();
        public List<Node> children = new List<Node>();
        public int[,] grid;
        public Pos player_pos;
        public Pos[] box_pos;
        public bool complete = false;
    }


    public void StartSolving(Room room)
    {
        Node node = new Node { grid = room.grid.Clone() as int[,] };
        node.box_pos = new Pos[room.num_boxes];
        node.box_pos = new Pos[room.num_boxes];
        button_pos = new Pos[room.num_boxes];
        int box_num = 0;
        int button_num = 0;

        for (int y = 0; y < node.grid.GetLength(1); y++)
        {
            for (int x = 0; x < node.grid.GetLength(0); x++)
            {
                if (node.grid[x, y] == (int)Elements.floor + (int)Elements.player)
                {
                    node.player_pos = new Pos { x = x, y = y };
                }
                if (node.grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    button_pos[button_num] = new Pos { x = x, y = y };
                    button_num++;
                }
                if (node.grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    node.box_pos[box_num] = new Pos { x = x, y = y };
                    box_num++;
                }
            }
        }

    }

    Direction GetClosestDir(Pos pos, Node node)
    {
        float distance = 1000;
        Direction closest = Direction.None;
        for (int i = 0; i < button_pos.Length; i++)
        {
            if (node.grid[button_pos[i].x, button_pos[i].y] == (int)Elements.floor + (int)Elements.button ||
                node.grid[button_pos[i].x, button_pos[i].y] == (int)Elements.floor + (int)Elements.button + (int)Elements.player)
            {
                for (Direction j = Direction.N; j < Direction.W; j++)
                {
                    if (j == Direction.N && GetDistance(new Pos { x = pos.x, y = pos.y + 1 }, button_pos[i]) < distance)
                    {
                        distance = GetDistance(new Pos { x = pos.x, y = pos.y + 1 }, button_pos[i]);
                        closest = j;
                    }
                    if (j == Direction.E && GetDistance(new Pos { x = pos.x + 1, y = pos.y }, button_pos[i]) < distance)
                    {
                        distance = GetDistance(new Pos { x = pos.x + 1, y = pos.y }, button_pos[i]);
                        closest = j;
                    }
                    if (j == Direction.S && GetDistance(new Pos { x = pos.x, y = pos.y - 1 }, button_pos[i]) < distance)
                    {
                        distance = GetDistance(new Pos { x = pos.x, y = pos.y - 1 }, button_pos[i]);
                        closest = j;
                    }
                    if (j == Direction.W && GetDistance(new Pos { x = pos.x - 1, y = pos.y }, button_pos[i]) < distance)
                    {
                        distance = GetDistance(new Pos { x = pos.x - 1, y = pos.y }, button_pos[i]);
                        closest = j;
                    }
                }
            }
        }

        return closest;
    }

    float GetDistance (Pos pos, Pos button)
    {
        float x_dist = pos.x - button.x, y_dist = pos.y - button.y;
        if (x_dist < 0) x_dist = -x_dist;
        if (y_dist < 0) y_dist = -y_dist;
        
        return x_dist + y_dist;
    }

    bool CheckCorner(Pos pos, Node node)
    {
        // Checks if box is in a corner (dead state)
        if ((node.grid[pos.x + 1, pos.y] == (int)Elements.wall && node.grid[pos.x, pos.y + 1] == (int)Elements.wall) ||
            (node.grid[pos.x + 1, pos.y] == (int)Elements.wall && node.grid[pos.x, pos.y - 1] == (int)Elements.wall) ||
            (node.grid[pos.x - 1, pos.y] == (int)Elements.wall && node.grid[pos.x, pos.y - 1] == (int)Elements.wall) ||
            (node.grid[pos.x - 1, pos.y] == (int)Elements.wall && node.grid[pos.x, pos.y + 1] == (int)Elements.wall))
        {
            return true;
        }
        return false;
    }

    bool MoveBox(int box_num, Direction dir, Node node)
    {
        Pos push_pos = null, to_pos = null;
        if (dir == Direction.N)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y - 1 };
            to_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y + 1 };
        }
        if (dir == Direction.E)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x - 1, y = node.box_pos[box_num].y };
            to_pos = new Pos { x = node.box_pos[box_num].x + 1, y = node.box_pos[box_num].y};
        }
        if (dir == Direction.S)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y + 1 };
            to_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y - 1 };
        }
        if (dir == Direction.W)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x + 1, y = node.box_pos[box_num].y };
            to_pos = new Pos { x = node.box_pos[box_num].x - 1, y = node.box_pos[box_num].y };
        }

        // Do not move box if:
        // Position to push from or position being pushed to are walls
        // There is no path from the player position to the push position (player cannot push in this direction)
        // The desired box position is a corner and is not a button (puts box in dead state, unless tile is a button)
        if (node.grid[push_pos.x, push_pos.y] == (int)Elements.wall || node.grid[to_pos.x, to_pos.y] == (int)Elements.wall ||
            !PathExists(push_pos, node.player_pos, node) || (node.grid[to_pos.x, to_pos.y] == (int)Elements.floor && CheckCorner(to_pos, node)))
        {
            return false;
        }
        else
        {
            // Moves box and player and creates new node
            Node new_node = new Node { parents = node.parents };
            new_node.parents.Add(node);
            node.children.Add(new_node);
            new_node.grid[new_node.box_pos[box_num].x, new_node.box_pos[box_num].y] -= (int)Elements.box;
            new_node.grid[new_node.player_pos.x, new_node.player_pos.y] -= (int)Elements.player;
            new_node.player_pos = new_node.box_pos[box_num];
            new_node.box_pos[box_num] = to_pos;
            new_node.grid[node.box_pos[box_num].x, new_node.box_pos[box_num].y] += (int)Elements.box;
            new_node.grid[node.player_pos.x, node.player_pos.y] += (int)Elements.player;
            return true;
        }
    }

    class PathCell
    {
        public Pos pos;
        public List<PathCell> children;
    }

    bool PathExists(Pos start, Pos player, Node node)
    {
        int[,] temp_grid = node.grid.Clone() as int[,];
        List<Pos> checked_floor = new List<Pos>();
        Direction dir = Direction.E;
        checked_floor.Add(start);

        // Loop until checks returns to start position (if it does, player cannot path to position)
        while (checked_floor.Count > 0)
        {
            bool placed = false;
            for (int i = 0; i < 4; i++)
            {
                Pos pos = CheckDir(checked_floor[checked_floor.Count - 1], dir, node);
                // If direction is free, add to checked list and set grid position to 0
                if (!pos.empty)
                {
                    // If player is found, path is available
                    if (temp_grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.player ||
                        temp_grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.player + (int)Elements.button)
                    {
                        return true;
                    }
                    temp_grid[pos.x, pos.y] = 0;
                    checked_floor.Add(pos);
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
        return false;
    }

    private Pos CheckDir(Pos pos, Direction dir, Node node)
    {
        if (dir == Direction.N && (node.grid[pos.x, pos.y + 1] == (int)Elements.floor)) return new Pos { x = pos.x, y = pos.y + 1 };
        if (dir == Direction.E && (node.grid[pos.x + 1, pos.y] == (int)Elements.floor)) return new Pos { x = pos.x + 1, y = pos.y };
        if (dir == Direction.S && (node.grid[pos.x, pos.y - 1] == (int)Elements.floor)) return new Pos { x = pos.x, y = pos.y - 1 };
        if (dir == Direction.W && (node.grid[pos.x - 1, pos.y] == (int)Elements.floor)) return new Pos { x = pos.x - 1, y = pos.y };
        return new Pos { empty = true };
    }
}
