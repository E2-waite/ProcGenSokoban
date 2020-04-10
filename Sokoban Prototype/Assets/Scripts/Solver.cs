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
        public bool complete = false;
    }

    public void StartSolving(Room room)
    {
        Node node = new Node { grid = room.grid.Clone() as int[,] };
        num_boxes = room.num_boxes;
        node.box_pos = new Pos[num_boxes];
        node.box_pos = new Pos[num_boxes];
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
        yield return new WaitForSeconds(0.001f);
        Debug.Log("STEP");
        //// If all buttons are pressed stop searching and set as solved
        //int buttons_pressed = 0;
        //for (int i = 0; i < num_boxes; i++)
        //{
        //    if (node.grid[button_pos[i].x, button_pos[i].y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
        //    {
        //        buttons_pressed++;
        //    }
        //    yield return null;
        //}
        //if (buttons_pressed == num_boxes)
        //{
        //    solved = true;
        //    yield break;
        //}

        Direction dir = last_dir;
        int box_num = last_box;
        //  Loops through all directions for each box attempting to move (starting with last moved box and direction)
        for (int i = 0; i < num_boxes; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Debug.Log("LOOPING");
                Node new_node = null;
                //Node new_node = MoveBox(box_num, dir, node);
                if (new_node != null)
                {
                    StartCoroutine(Step(new_node, dir, box_num));
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

        // If reaches this point, none of the boxes can be moved in any direction (step back)
        node.complete = true;
        if (node.parents.Count > 0)
        {
            Step(node.parents[node.parents.Count - 1], dir, box_num);
        }
        else
        {
            failed = true;
        }
    }

    Direction GetClosestDir(Pos pos, Node node)
    {
        Debug.Log("Getting Closest Dir");
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

    IEnumerator MoveBox(int box_num, Direction dir, Node node)
    {
        Debug.Log("Moving Box");
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

        if (dir == Direction.N)
        {
            dir = Direction.W;
        }
        else
        {
            dir++;
        }

        // Do not move box if:
        // Position to push from or position being pushed to are walls
        // There is no path from the player position to the push position (player cannot push in this direction)
        // The desired box position is a corner and is not a button (puts box in dead state, unless tile is a button)
        // One of this node's children moved to to_pos (already attempted)
        if (node.grid[push_pos.x, push_pos.y] == (int)Elements.wall || node.grid[to_pos.x, to_pos.y] == (int)Elements.wall ||
            !PathExists(push_pos, node) || (node.grid[to_pos.x, to_pos.y] == (int)Elements.floor && CheckCorner(to_pos, node)))
        {
            StartCoroutine(Step(node, dir, box_num));
            yield break;
        }
        
        for (int i = 0; i < node.children.Count; i++)
        {
            if (node.children[i].box_pos[box_num].x == to_pos.x &&
                node.children[i].box_pos[box_num].y == to_pos.y)
            {
                // Prevents checking the same child node multiple times
                StartCoroutine(Step(node, dir, box_num));
                yield break;
            }
            yield return null;
        }

        for (int i = node.parents.Count - 1; i >= 0; i--)
        {
            if (node.parents[i].box_pos[box_num].x == to_pos.x &&
                node.parents[i].box_pos[box_num].y == to_pos.y)
            {
                StartCoroutine(Step(node, dir, box_num));
                yield break;
            }
            yield return null;
        }

        // Moves box and player and creates new node
        Node new_node = new Node { grid = node.grid.Clone() as int[,], parents = node.parents };
        new_node.parents.Add(node);
        node.children.Add(new_node);
        new_node.grid[new_node.box_pos[box_num].x, new_node.box_pos[box_num].y] -= (int)Elements.box;
        new_node.grid[new_node.player_pos.x, new_node.player_pos.y] -= (int)Elements.player;
        new_node.player_pos = new_node.box_pos[box_num];
        new_node.box_pos[box_num] = to_pos;
        new_node.grid[node.box_pos[box_num].x, new_node.box_pos[box_num].y] += (int)Elements.box;
        new_node.grid[node.player_pos.x, node.player_pos.y] += (int)Elements.player;
        StartCoroutine(Step(new_node, dir, box_num));
    }

    class PathCell
    {
        public Pos pos;
        public List<PathCell> children;
    }

    bool PathExists(Pos start, Node node)
    {
        Debug.Log("Checking Player to Box Path");
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
