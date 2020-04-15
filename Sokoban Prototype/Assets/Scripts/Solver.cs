using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Linq;
using System;
using enums;
public class Solver : MonoBehaviour
{
    public int num_moves = 0;
    public int max_steps;
    int num_boxes;
    class Move
    {
        public int box_num;
        public Direction dir;
        public Pos pos;
    }

    class Node
    {
        public List<Node> parents = new List<Node>();
        public List<Node> children = new List<Node>();
        public int[,] grid;
        public Pos player_pos;
        public Pos[] box_pos;
        public Pos[] button_pos;
        public DirsChecked[] dirs;
        public bool complete = false, setup = false;
        public Move move;
        public float distance;
    }

    class DirsChecked
    {
        public bool[] _checked = new bool[4];
    }

    public void StartSolving(Room room, Attempt attempt)
    {
        Debug.Log("STARTING SOLVER");
        num_boxes = room.num_boxes;
        Node node = new Node { grid = room.grid.Clone() as int[,], player_pos = new Pos { x = room.entrance.x, y = room.entrance.y },
            box_pos = new Pos[num_boxes], button_pos = new Pos[num_boxes], dirs = new DirsChecked[num_boxes] };
        node.move = new Move { box_num = 0, dir = Direction.None, pos = new Pos { x = 0, y = 0 } };
        for (int i = 0; i < node.dirs.Length; i++)
        {
            node.dirs[i] = new DirsChecked();
        }

        StartCoroutine(StartStep(node, attempt));
    }

    IEnumerator StartStep(Node node, Attempt attempt)
    {
        max_steps = (int)(Mathf.Sqrt((node.grid.GetLength(0) - 2) * (node.grid.GetLength(1) - 2))) * 10;
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
                    node.button_pos[button_num] = new Pos { x = x, y = y };
                    button_num++;
                }
                if (node.grid[x, y] == (int)Elements.floor + (int)Elements.box)
                {
                    node.box_pos[box_num] = new Pos { x = x, y = y };
                    box_num++;
                }
                yield return null;
            }
        }
        SortByDistance(node);
        StartCoroutine(Step(node, attempt));
    }

    bool SortByDistance(Node node)
    {
        int[] targets = new int[node.box_pos.Length];
        for (int i = 0; i < node.box_pos.Length; i++)
        {
            int dist = 1000;
            for (int j = 0; j < node.button_pos.Length; j++)
            {
                int dist_x = node.box_pos[i].x - node.button_pos[i].x;
                int dist_y = node.box_pos[i].y - node.button_pos[i].y;
                if (dist_x < 0)
                {
                    dist_x = -dist_x;
                }
                if (dist_y < 0)
                {
                    dist_y = -dist_y;
                }
                int this_dist = dist_x + dist_y;
                if (this_dist < dist && !targets.Contains(j))
                {
                    dist = this_dist;
                    targets[i] = j;
                }
            }
        }
        Array.Sort(targets, node.button_pos);
        return true;
    }

    float GetDistance(Node node)
    {
        int distance = 0;
        for (int i = 0; i < node.box_pos.Length; i++)
        {
            int dist_x = node.box_pos[i].x - node.button_pos[i].x;
            int dist_y = node.box_pos[i].y - node.button_pos[i].y;
            if (dist_x < 0)
            {
                dist_x = -dist_x;
            }
            if (dist_y < 0)
            {
                dist_y = -dist_y;
            }
            distance += dist_x + dist_y;
        }
        return distance;
    }

    IEnumerator Step(Node node, Attempt attempt)
    {
        string line = null;
        for (int y = 0; y < node.grid.GetLength(1); y++)
        {
            for (int x = 0; x < node.grid.GetLength(0); x++)
            {
                if (node.grid[x, y] == (int)Elements.floor + (int)Elements.box ||
                    node.grid[x, y] == (int)Elements.trapdoor + (int)Elements.box ||
                    node.grid[x, y] == (int)Elements.floor + (int)Elements.box + (int)Elements.button)
                {
                    line += "B ";
                }
                else if (node.grid[x, y] == (int)Elements.floor + (int)Elements.player ||
                    node.grid[x, y] == (int)Elements.trapdoor + (int)Elements.player ||
                    node.grid[x, y] == (int)Elements.floor + (int)Elements.player + (int)Elements.button)
                {
                    line += "P ";
                }
                else if (node.grid[x, y] == (int)Elements.floor + (int)Elements.button)
                {
                    line += "X ";
                }
                else if (node.grid[x, y] == (int)Elements.floor || node.grid[x, y] == (int)Elements.trapdoor)
                {
                    line += "   ";                 
                }
                else if (node.grid[x, y] == (int)Elements.wall)
                {
                    line += "0 ";
                }
                else
                {
                    line += "? ";
                }
            }
            line += "\n";
        }

        
        Debug.Log(line);
        int buttons_pressed = 0;
        for (int i = 0; i < num_boxes; i++)
        {
            if (node.grid[node.button_pos[i].x, node.button_pos[i].y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
            {
                buttons_pressed++;
            }
            yield return null;
        }
        if (buttons_pressed > 0)
        {
            //Debug.Log("Buttons Pressed: " + buttons_pressed.ToString() + " Depth: " + node.parents.Count.ToString());
        }
        if (buttons_pressed == num_boxes)
        {
            attempt.solved = true;
            yield break;
        }


        if (node.parents.Count < max_steps)
        {
            // If node has no children setup all children and attempt movement
            if (node.children.Count == 0)
            {
                for (int i = 0; i < num_boxes; i++)
                {
                    for (Direction j = Direction.N; j < (Direction)4; j++)
                    {
                        node.children.Add(new Node());
                        StartCoroutine(AttemptMovement(i, j, node, node.children[node.children.Count - 1]));
                    }
                }

                int num_setup = 0;
                while (num_setup != node.children.Count)
                {
                    num_setup = 0;
                    for (int i = 0; i < node.children.Count; i++)
                    {
                        if (node.children[i].setup)
                        {
                            num_setup++;
                        }
                        yield return null;
                    }
                }
                node.children = node.children.OrderBy(w => w.distance).ToList();
            }
            for (int i = 0; i < node.children.Count; i++)
            {
                if (!node.children[i].complete)
                {
                    StartCoroutine(Step(node.children[i], attempt));
                    yield break;
                }
            }
        }

        node.complete = true;
        if (node.parents.Count > 0)
        {
            StartCoroutine(Step(node.parents[node.parents.Count - 1], attempt));
        }
        else
        {
            attempt.failed = true;
        }
    }

    IEnumerator AttemptMovement(int num, Direction dir, Node node, Node new_node)
    {
        Pos push_pos = null, to_pos = null, player_pos = null;
        if (dir == Direction.N)
        {
            push_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y - 1 };
            player_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y + 1 };
        }
        if (dir == Direction.E)
        {
            push_pos = new Pos { x = node.box_pos[num].x - 1, y = node.box_pos[num].y };
            player_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x + 1, y = node.box_pos[num].y };
        }
        if (dir == Direction.S)
        {
            push_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y + 1 };
            player_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y - 1 };
        }
        if (dir == Direction.W)
        {
            push_pos = new Pos { x = node.box_pos[num].x + 1, y = node.box_pos[num].y };
            player_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x - 1, y = node.box_pos[num].y };
        }

        new_node.move = new Move { box_num = num, dir = dir, pos = to_pos };

        if (!CheckPos(push_pos, node) || !CheckPos(to_pos, node) ||
            (node.grid[to_pos.x, to_pos.y] == (int)Elements.floor && CheckCorner(to_pos, node)))
        {
            new_node.complete = true;
        }
        yield return null;


        if (!new_node.complete)
        {
            for (int i = node.parents.Count - 1; i >= 0; i--)
            {
                if (node.parents[i].move.box_num == num &&
                    node.parents[i].move.dir == dir &&
                    node.parents[i].move.pos.x == to_pos.x &&
                    node.parents[i].move.pos.y == to_pos.y)
                {
                    new_node.complete = true;
                    break;
                }
                yield return null;
            }
        }

        if (!new_node.complete)
        {
            // Move box and player to new position
            new_node.grid = node.grid.Clone() as int[,];
            new_node.box_pos = new Pos[num_boxes];
            new_node.player_pos = new Pos { x = node.player_pos.x, y = node.player_pos.y };
            new_node.button_pos = node.button_pos;
            new_node.parents = new List<Node>(node.parents);
            new_node.parents.Add(node);
            for (int i = 0; i < num_boxes; i++)
            {
                new_node.box_pos[i] = new Pos { x = node.box_pos[i].x, y = node.box_pos[i].y };
                yield return null;
            }
            new_node.grid[node.box_pos[num].x, node.box_pos[num].y] -= (int)Elements.box;
            new_node.grid[node.player_pos.x, node.player_pos.y] -= (int)Elements.player;
            new_node.player_pos = player_pos;
            new_node.box_pos[num] = to_pos;
            new_node.grid[new_node.box_pos[num].x, new_node.box_pos[num].y] += (int)Elements.box;
            new_node.grid[new_node.player_pos.x, new_node.player_pos.y] += (int)Elements.player;
            new_node.distance = GetDistance(new_node);
        }
        new_node.setup = true;
    }

    bool CheckPos(Pos pos, Node node)
    {
        if (node.grid[pos.x, pos.y] == (int)Elements.wall ||
            node.grid[pos.x, pos.y] == (int)Elements.entrance ||
            node.grid[pos.x, pos.y] == (int)Elements.exit ||
            node.grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.box ||
            node.grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.box + (int)Elements.button)
        {
            return false;
        }
        return true;
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
}
