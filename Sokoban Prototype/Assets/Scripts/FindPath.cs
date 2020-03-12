using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class FindPath
{
    Node[,] node_grid;
    public List<Pos> final_path = new List<Pos>();
    public bool path_failed = false;
    FindPath(int[,] grid, Pos start_pos, Pos end_pos)
    {
        int grid_x = grid.GetLength(0), grid_y = grid.GetLength(1);
        node_grid = new Node[grid_x, grid_y];

        // Create node grid
        for (int y = 0; y < grid_y; y++)
        {
            for (int x = 0; x < grid_x; x++)
            {
                bool is_obstacle = false;
                if (grid[x, y] == (int)Elements.wall || grid[x,y] == (int)Elements.floor + (int)Elements.button)
                {
                    is_obstacle = true;
                }
                node_grid[x, y] = new Node(is_obstacle, new Pos { x = x, y = y });
            }
        }

        GeneratePath(start_pos, end_pos);
    }

    void GeneratePath(Pos start_pos, Pos end_pos)
    {
        int num_steps = 0;
        Node start_node = node_grid[start_pos.x, start_pos.y];
        Node end_node = node_grid[end_pos.x, end_pos.y];

        List<Node> open_list = new List<Node>();
        HashSet<Node> closed_list = new HashSet<Node>();

        open_list.Add(start_node);
        
        while (open_list.Count > 0)
        {
            // Looks at node at the bottom of the open list
            Node current_node = open_list[0];

            // Then look through all the following nodes in the open list
            for (int i = 1; i < open_list.Count; i++)
            {
                if ((open_list[i].f_cost < current_node.f_cost || open_list[i].f_cost == current_node.f_cost) && open_list[i].h_cost < current_node.h_cost)
                {
                    // If open list F Cost and H Cost is lower than the current node, it is closer to the goal
                    current_node = open_list[i];
                }
            }

            open_list.Remove(current_node);
            closed_list.Add(current_node);

            if (current_node == end_node)
            {
                // Path has been found
                GetFinalPath(start_node, end_node);
            }

            foreach (Node neighbour in GetNeighbourNodes(current_node))
            {
                // If the neighbour is a wall, or is on the closed list (already checked) skip over it
                if (neighbour.is_wall || closed_list.Contains(neighbour))
                {
                    continue;
                }

                int move_cost = current_node.g_cost + GetManhattenDistance(current_node, neighbour);
                if (move_cost < neighbour.g_cost || !open_list.Contains(neighbour))
                {
                    neighbour.g_cost = move_cost;
                    neighbour.h_cost = GetManhattenDistance(neighbour, end_node);
                    neighbour.parent = current_node;

                    if (!open_list.Contains(neighbour))
                    {
                        open_list.Add(neighbour);
                    }
                }
            }

            // If number of steps exceeds 100, a path probably cannot be found
            num_steps++;
            if (num_steps > 100)
            {
                path_failed = false;
                break;
            }
        }
    }

    int GetManhattenDistance(Node node_a, Node node_b)
    {
        int x_dist = Mathf.Abs(node_a.pos.x - node_b.pos.x);
        int y_dist = Mathf.Abs(node_a.pos.y - node_b.pos.y);
        return x_dist + y_dist;
    }

    List<Node> GetNeighbourNodes(Node node)
    {
        // Gets all neighbouring nodes (ensuring none are outside of the grid)
        List<Node> neighbour_nodes = new List<Node>();
        
        // Check up
        if (node.pos.x >= 0 && node.pos.x< node_grid.GetLength(0) &&
            node.pos.y - 1 >= 0 && node.pos.y - 1 < node_grid.GetLength(1))
        {
            neighbour_nodes.Add(node_grid[node.pos.x, node.pos.y - 1]);
        }
        // Check right
        if (node.pos.x + 1 >= 0 && node.pos.x + 1 < node_grid.GetLength(0) &&
            node.pos.y >= 0 && node.pos.y < node_grid.GetLength(1))
        {
            neighbour_nodes.Add(node_grid[node.pos.x + 1, node.pos.y]);
        }
        // Check down
        if (node.pos.x >= 0 && node.pos.x < node_grid.GetLength(0) &&
            node.pos.y + 1 >= 0 && node.pos.y + 1 < node_grid.GetLength(1))
        {
            neighbour_nodes.Add(node_grid[node.pos.x, node.pos.y + 1]);
        }
        // Check left
        if (node.pos.x - 1 >= 0 && node.pos.x - 1 < node_grid.GetLength(0) &&
            node.pos.y >= 0 && node.pos.y < node_grid.GetLength(1))
        {
            neighbour_nodes.Add(node_grid[node.pos.x - 1, node.pos.y]);
        }

        return neighbour_nodes;
    }

    void GetFinalPath(Node start_node, Node end_node)
    {
        List<Pos> pos_path = new List<Pos>();
        Node current_node = end_node;

        // Work backwards from the end node to the start node to create the final path
        while (current_node != start_node)
        {
            pos_path.Add(current_node.pos);
            current_node = current_node.parent;
        }

        // Path needs to be flipped (worked backwards from the end)
        pos_path.Reverse();
        final_path = pos_path;
    }
}
