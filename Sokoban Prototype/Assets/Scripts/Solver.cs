using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Solver : MonoBehaviour
{
    public bool solved = false;
    public bool failed = false;
    public int num_moves = 0;
    Pos[] button_pos;
    int num_boxes;
    class Node
    {
        public List<Node> parents = new List<Node>();
        public List<Node> children = new List<Node>();
        public int[,] grid;
        public Pos player_pos;
        public Pos[] box_pos;
        public DirsChecked[] dirs;
        public bool complete = false;
    }

    class DirsChecked
    {
        public bool[] _checked = new bool[4];
    }

    public void StartSolving(Room room)
    {
        Debug.Log("STARTING");
        num_boxes = room.num_boxes;
        Node node = new Node { grid = room.grid.Clone() as int[,], box_pos = new Pos[num_boxes],  dirs = new DirsChecked[num_boxes] };
        for (int i = 0; i < node.dirs.Length; i++)
        {
            node.dirs[i] = new DirsChecked();
        }
        button_pos = new Pos[num_boxes];
        StartCoroutine(StartStep(node));
    }

    IEnumerator StartStep(Node node)
    {
        Debug.Log("STARTED STEP");
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
                yield return null;
            }
        }
        Debug.Log("FINISHED START STEP");
        Direction dir = GetClosestDir(node.box_pos[0], node);
        StartCoroutine(Step(node, dir, 0));
    }

    IEnumerator Step(Node node, Direction last_dir, int last_box)
    {
        Debug.Log(node.parents.Count.ToString());
        //Debug.Log("0: " + node.box_pos[0].x.ToString() + " " + node.box_pos[0].y.ToString() +
        //         " 1: " + node.box_pos[1].x.ToString() + " " + node.box_pos[1].y.ToString() +
        //         " 2: " + node.box_pos[2].x.ToString() + " " + node.box_pos[2].y.ToString());
        // If all buttons are pressed stop searching and set as solved
        int buttons_pressed = 0;
        for (int i = 0; i < num_boxes; i++)
        {
            if (node.grid[button_pos[i].x, button_pos[i].y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
            {
                buttons_pressed++;
            }
            yield return null;
        }
        if (buttons_pressed > 0)
        {
            Debug.Log("Buttons Pressed: " + buttons_pressed.ToString());
        }
        if (buttons_pressed == num_boxes)
        {
            solved = true;
            yield break;
        }

        Direction dir = last_dir;
        int box_num = last_box;
        //  Loops through all directions for each box attempting to move (starting with last moved box and direction)
        for (int i = 0; i < num_boxes; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (!node.dirs[box_num]._checked[(int)dir])
                {
                    StartCoroutine(CheckMovement(box_num, dir, node));
                    yield break;
                }

                if (dir == Direction.W)
                {
                    dir = Direction.N;
                }
                else
                {
                    dir++;
                }
                yield return null;
            }
            if (box_num == num_boxes - 1)
            {
                box_num = 0;
            }
            else
            {
                box_num++;
            }
            // When switching to next box, find the closest direction for that box
            dir = GetClosestDir(node.box_pos[box_num], node);
        }

        Debug.Log("BACK, Node parents: " + node.parents.Count.ToString());
        // If reaches this point, none of the boxes can be moved in any direction (step back)
        node.complete = true;
        if (node.parents.Count > 0)
        {
            StartCoroutine(Step(node.parents[node.parents.Count - 1], dir, box_num));
        }
        else
        {
            failed = true;
            yield break;
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

    class Move
    {
        public int box_num;
        public Pos pos;
        public Direction dir;
        public bool can_move;
    }


    IEnumerator CheckMovement(int box_num, Direction dir, Node node)
    {
        node.dirs[box_num]._checked[(int)dir] = true;
        Pos push_pos = null, to_pos = null;
        if (dir == Direction.N)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y - 1 };
            to_pos = new Pos { x = node.box_pos[box_num].x, y = node.box_pos[box_num].y + 1 };
        }
        if (dir == Direction.E)
        {
            push_pos = new Pos { x = node.box_pos[box_num].x - 1, y = node.box_pos[box_num].y };
            to_pos = new Pos { x = node.box_pos[box_num].x + 1, y = node.box_pos[box_num].y };
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
        // One of this node's children moved to to_pos (already attempted)
        if (node.grid[push_pos.x, push_pos.y] == (int)Elements.wall || node.grid[push_pos.x, push_pos.y] == (int)Elements.entrance ||
            node.grid[push_pos.x, push_pos.y] == (int)Elements.exit || node.grid[to_pos.x, to_pos.y] == (int)Elements.wall ||
            node.grid[to_pos.x, to_pos.y] == (int)Elements.entrance || node.grid[to_pos.x, to_pos.y] == (int)Elements.exit ||
            (node.grid[to_pos.x, to_pos.y] == (int)Elements.floor && CheckCorner(to_pos, node)))
        {
            StartCoroutine(Step(node, dir, box_num));
            yield break;
        }

        for (int i = node.parents.Count - 1; i >= 0; i--)
        {
            if (node.parents[i].box_pos[box_num].x == to_pos.x &&
                node.parents[i].box_pos[box_num].y == to_pos.y)
            {
                //Debug.Log("STEPPED BEFORE");
                StartCoroutine(Step(node, dir, box_num));
                yield break;
            }
            yield return null;
        }

        StartCoroutine(MoveBox(box_num, dir, to_pos, node));
        yield break;
    }

    IEnumerator MoveBox(int box_num, Direction dir, Pos pos, Node node)
    {
        //Debug.Log("MOVING BOX");
        // Moves box and player and creates new node
        Node new_node = new Node 
        { 
            dirs = new DirsChecked[num_boxes], 
            grid = node.grid.Clone() as int[,],
            box_pos = new Pos[num_boxes], 
            player_pos = new Pos { x = node.player_pos.x, y = node.player_pos.y }

        };
        for (int i = 0; i < num_boxes; i++)
        {
            new_node.box_pos[i] = new Pos { x = node.box_pos[i].x, y = node.box_pos[i].y };
        }
        for (int i = 0; i < new_node.dirs.Length; i++)
        {
            new_node.dirs[i] = new DirsChecked();
            yield return null;
        }
        new_node.parents = new List<Node>();
        for (int i = 0; i < node.parents.Count; i++)
        {
            new_node.parents.Add(node.parents[i]);
            yield return null;
        }
        new_node.parents.Add(node);
        node.children.Add(new_node);
        new_node.grid[new_node.box_pos[box_num].x, new_node.box_pos[box_num].y] -= (int)Elements.box;
        new_node.grid[new_node.player_pos.x, new_node.player_pos.y] -= (int)Elements.player;
        new_node.player_pos = new_node.box_pos[box_num];
        new_node.box_pos[box_num] = new Pos { x = pos.x, y = pos.y};
        new_node.grid[node.box_pos[box_num].x, new_node.box_pos[box_num].y] += (int)Elements.box;
        new_node.grid[node.player_pos.x, node.player_pos.y] += (int)Elements.player;
        StartCoroutine(Step(new_node, dir, box_num));
    }

    class PathCell
    {
        public Pos pos;
        public List<PathCell> children;
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
