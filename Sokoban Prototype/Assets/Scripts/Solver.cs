using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using enums;
public class Solver : MonoBehaviour
{
    public int num_moves = 0;
    public int max_depth;
    float max_timeout = 5.0f;
    int num_boxes;
    bool started = false;
    float time;

    class Node
    {
        public int[,] grid;
        public Pos player_pos;
        public Pos[] box_pos;
        public Pos[] button_pos;
        public int cost;
        public int depth = 0;
    }

    class DirsChecked
    {
        public bool[] _checked = new bool[4];
    }

    private void Update()
    {
        if (started)
        {
            time += Time.deltaTime;
        }
    }

    public void StartSolving(Room room, Attempt attempt)
    {
        started = true;
        time = 0;
        num_boxes = room.num_boxes;
        Node node = new Node { grid = room.grid.Clone() as int[,], player_pos = new Pos { x = room.entrance.x, y = room.entrance.y },
            box_pos = new Pos[num_boxes], button_pos = new Pos[num_boxes] };

        StartCoroutine(StartStep(node, attempt));
    }

    IEnumerator StartStep(Node node, Attempt attempt)
    {
        max_depth = (int)(Mathf.Sqrt((node.grid.GetLength(0) - 2) * (node.grid.GetLength(1) - 2))) * 2;
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
        StartCoroutine(GetDeadCells(node, attempt));
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

    IEnumerator GetDeadCells(Node node, Attempt attempt)
    {
        List<Pos> corners = new List<Pos>();
        // If tile is floor and is in a wall corner, it is marked as a dead square
        for (int y = node.grid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < node.grid.GetLength(0); x++)
            {
                if (node.grid[x,y] == (int)Elements.floor && IsCorner(new Pos { x = x, y = y }, node))
                {
                    corners.Add(new Pos { x = x, y = y });
                    node.grid[x, y] = (int)Elements.dead;
                }
                yield return null;
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
                            if (!CheckWall(pos, node) || 
                                node.grid[pos.x, pos.y] == (int)Elements.floor + (int)Elements.button ||
                                node.grid[pos.x, pos.y] == (int)Elements.wall)
                            {
                                fill = false;
                                break;
                            }

                            spaces.Add(new Pos { x = pos.x, y = pos.y });
                            yield return null;
                        }


                        if (fill)
                        {
                            string row = "Row: ";
                            for (int k = 0; k < spaces.Count; k++)
                            {
                                row += " x:" + spaces[k].x.ToString() + " y:" + spaces[k].y.ToString() + " |";
                                node.grid[spaces[k].x, spaces[k].y] = (int)Elements.dead;
                                yield return null;
                            }
                        }
                    }
                }
            }
            corners.Remove(corners[i]);
        }

        for (int i = 0; i < node.box_pos.Length; i++)
        {
            if (node.grid[node.box_pos[i].x, node.box_pos[i].x] == (int)Elements.dead)
            {
                attempt.failed = true;
                Debug.Log("BOX IN DEAD STATE, FAILED");
                yield break;
            }
        }

        StartCoroutine(Solve(node, attempt));
    }

    bool CheckWall(Pos pos, Node node)
    {
        if (node.grid[pos.x, pos.y + 1] == (int)Elements.wall ||
            node.grid[pos.x + 1, pos.y] == (int)Elements.wall || 
            node.grid[pos.x, pos.y - 1] == (int)Elements.wall ||
            node.grid[pos.x - 1, pos.y] == (int)Elements.wall)
        {
            return true;
        }
        return false;
    }

    int GetDistance(Node node)
    {
        int distance = 1000;
        for (int i = 0; i < node.box_pos.Length; i++)
        {
            for (int j = 0; j < node.button_pos.Length; j++)
            {
                int dist_x = node.box_pos[i].x - node.button_pos[j].x;
                int dist_y = node.box_pos[i].y - node.button_pos[j].y;
                if (dist_x < 0)
                {
                    dist_x = -dist_x;
                }
                if (dist_y < 0)
                {
                    dist_y = -dist_y;
                }
                if (dist_x + dist_y < distance)
                {
                    distance = dist_x + dist_y;
                }
            }
        }
        return distance;
    }

    IEnumerator Solve(Node start_node, Attempt attempt)
    {
        List<Node> open_list = new List<Node>();
        List<Node> closed_list = new List<Node>();
        open_list.Add(start_node);

        while (open_list.Count > 0)
        {
            Debug.Log("LOOPING");
            Node current_node = open_list[0];
            open_list.Remove(current_node);
            closed_list.Add(current_node);
            int buttons_pressed = 0;

            foreach (Pos box_pos in current_node.box_pos)
            {
                if (current_node.grid[box_pos.x, box_pos.y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
                {
                    buttons_pressed++;
                }
            }
            if (buttons_pressed == num_boxes)
            {
                Debug.Log("SOLVED");
                attempt.solved = true;
                break;
            }
            else
            {
                if (current_node.depth >= max_depth  || Deadlock(current_node))
                {
                    // If deadlock, or depth > max depth, or time > max time, continue to next solution
                    continue;
                }
                else
                {
                    // Get valid moves
                    List<Node> moves = new List<Node>();
                    for (int i = 0; i < num_boxes; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Node move = CanMove(current_node, (Direction)j, i);
                            if (move != null)
                            {
                                moves.Add(move);
                            }
                            yield return null;
                        }
                    }

                    foreach (Node move in moves)
                    {
                        // Add cost to current move
                        // Update the queue with new cost and new state
                        // Find heuristics of new state
                        if (!closed_list.Contains(move))
                        {
                            int dist = GetDistance(move);
                            current_node.cost += dist;
                            if (!open_list.Contains(move))
                            {
                                open_list.Add(move);
                            }
                            move.cost = dist;
                        }
                    }
                }
                open_list = open_list.OrderBy(w => w.cost).ToList();
            }
        }

        if (!attempt.solved)
        {
            attempt.failed = true;
        }
    }

    bool Deadlock(Node node)
    {
        if (FreezeDeadlock(node))
        {
            return true;
        }
        return false;
    }

    bool FreezeDeadlock(Node node)
    {
        for (int i = 0; i < num_boxes; i++)
        {
            if (node.grid[node.box_pos[i].x, node.box_pos[i].y] != (int)Elements.floor + (int)Elements.button + (int)Elements.box &&
                IsCorner(node.box_pos[i], node))
            {
                return true;
            }
        }
        return false;
    }

    Node CanMove(Node node, Direction dir, int num)
    {
        Pos push_pos = null, to_pos = null;
        if (dir == Direction.N)
        {
            push_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y - 1 };
            to_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y + 1 };
        }
        if (dir == Direction.E)
        {
            push_pos = new Pos { x = node.box_pos[num].x - 1, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x + 1, y = node.box_pos[num].y };
        }
        if (dir == Direction.S)
        {
            push_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y + 1 };
            to_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y - 1 };
        }
        if (dir == Direction.W)
        {
            push_pos = new Pos { x = node.box_pos[num].x + 1, y = node.box_pos[num].y };
            to_pos = new Pos { x = node.box_pos[num].x - 1, y = node.box_pos[num].y };
        }

        if (TileFree(push_pos, node) && TileFree(to_pos, node) && !IsCorner(to_pos, node))
        {
            Node new_node = new Node
            {
                box_pos = node.box_pos.Clone() as Pos[],
                button_pos = node.button_pos.Clone() as Pos[],
                player_pos = new Pos { x = node.box_pos[num].x, y = node.box_pos[num].y },
                grid = node.grid.Clone() as int[,],
                depth = node.depth + 1
            };
            new_node.box_pos[num] = to_pos;
            new_node.grid[node.player_pos.x, node.player_pos.y] -= (int)Elements.player;
            new_node.grid[new_node.player_pos.x, new_node.player_pos.y] += (int)Elements.player;
            new_node.grid[node.box_pos[num].x, node.box_pos[num].y] -= (int)Elements.box;
            new_node.grid[new_node.box_pos[num].x, new_node.box_pos[num].y] += (int)Elements.box;
            return new_node;
        }
        else
        {
            return null;
        }
    }    

    bool TileFree(Pos pos, Node node)
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

    bool IsDead(Pos pos, Node node)
    {
        if (node.grid[pos.x, pos.y] == (int)Elements.dead ||
            node.grid[pos.x, pos.y] == (int)Elements.dead + (int)Elements.player)
        {
            return true;
        }
        return false;
    }

    bool OnButton(int num, Node node)
    {
        for (int i = 0; i < node.button_pos.Length; i++)
        {
            if (node.box_pos[num].x == node.button_pos[i].x &&
                node.box_pos[num].y == node.button_pos[i].y)
            {
                return true;
            }
        }
        return false;
    }

    bool IsCorner(Pos pos, Node node)
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
            if (node.grid[new_pos.x, new_pos.y] == (int)Elements.wall ||
                node.grid[new_pos.x, new_pos.y] == (int)Elements.floor + (int)Elements.box ||
                node.grid[new_pos.x, new_pos.y] == (int)Elements.floor + (int)Elements.box + (int)Elements.button ||
                node.grid[new_pos.x, new_pos.y] == (int)Elements.trapdoor + (int)Elements.box)
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
}
