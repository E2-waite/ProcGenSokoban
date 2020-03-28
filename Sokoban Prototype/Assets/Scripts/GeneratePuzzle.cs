using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GeneratePuzzle : MonoBehaviour
{
    public int num_boxes = 3, min_steps = 8, max_attempts = 10;
    int[,] empty_grid;
    int attempts = 0, running = 0;
    List<Pos> button_positions;
    List<Pos> box_positions;
    public void Generate(Room room)
    {
        attempts = 0;
        empty_grid = room.grid.Clone() as int[,];
        PlaceButtons(room);
    }

    private void PlaceButtons(Room room)
    {
        int[, ] grid = room.grid.Clone() as int[,];
        if (attempts > max_attempts)
        {
            NewRoom(room);
        }
        button_positions = new List<Pos>();
        // Place buttons in valid floor tile positions
        while (button_positions.Count < num_boxes)
        {
            int x_pos = Random.Range(1, grid.GetLength(0) - 1);
            int y_pos = Random.Range(1, grid.GetLength(1) - 1);
            if (grid[x_pos, y_pos] == (int)Elements.floor)
            {
                grid[x_pos, y_pos] += (int)Elements.button;
                button_positions.Add(new Pos { x = x_pos, y = y_pos });
            }
        }
        Debug.Log("Buttons Placed");
        box_positions = new List<Pos>();
        BoxPlace(room);
        attempts++;
    }
    
    class Node
    {
        public Pos pos;
        public List<Node> children = new List<Node>();
        public List<Node> stepped = new List<Node>();
        public bool complete = false;
    }

    void BoxPlace(Room room)
    {
        StopAllCoroutines();
        // If enough boxes have been placed, continue else place next box
        if (box_positions.Count == num_boxes)
        {
            // If the floor is continuous, begin generation else start new configuration
            GridCheck check = new GridCheck(room.grid);
            if (check.FloorCount() && check.ContinuousFloor())
            {
                for (int i = 0; i < button_positions.Count; i++)
                {
                    room.grid[button_positions[i].x, button_positions[i].y] = (int)Elements.floor + (int)Elements.button;
                }
                GetComponent<GenerateObjects>().Generate(room);
            }
            else
            {
                room.grid = empty_grid;
                PlaceButtons(room);
            }
        }
        else
        {
            // If there are still boxes to place, begin checking for next box
            Node current_node = new Node { pos = button_positions[box_positions.Count] };
            deepest_node = current_node;
            StartCoroutine(CheckNode(current_node, room));
        }
    }

    Node deepest_node;

    IEnumerator CheckNode(Node current_node, Room room)
    {
        running++;
        current_node.stepped.Add(current_node);
        if (current_node.stepped.Count >  deepest_node.stepped.Count)
        {
            deepest_node = current_node;
        }
        Direction dir = Direction.N;
        if (current_node.children.Count == 0)
        {
            // Get all valid neighbours, add them to children of current node
            for (int i = 0; i < 4; i++)
            {
                Pos checked_pos = CheckDir(dir, room.grid, current_node.pos);
                bool stepped = false;
                if (!checked_pos.empty)
                {
                    for (int j = 0; j < current_node.stepped.Count; j++)
                    {
                        if (current_node.stepped[j].pos.x == checked_pos.x &&
                            current_node.stepped[j].pos.y == checked_pos.y)
                        {
                            // if node has been stepped to previously in the tree, do not add as child
                            stepped = true;
                            break;
                        }
                        yield return null;
                    }
                    if (!stepped && room.grid[checked_pos.x, checked_pos.y] != (int)Elements.floor + (int)Elements.button)
                    {
                        current_node.children.Add(new Node { pos = checked_pos, stepped = current_node.stepped });
                    }
                }
                if (dir == Direction.W) dir = Direction.N;
                else dir++;
            }
        }

        // Check child nodes
        int num_complete = 0;
        if (current_node.children.Count > 0)
        {
            for (int i = 0; i < current_node.children.Count; i++)
            {
                if (current_node.children[i].complete)
                {
                    num_complete++;
                }
                else
                {
                    StartCoroutine(CheckNode(current_node.children[i], room));
                }
                yield return null;
            }
        }
        if (num_complete == current_node.children.Count)
        {
            current_node.complete = true;
        }

        if (current_node.children.Count == 0 || num_complete == current_node.children.Count)
        {
            Debug.Log("REACHED END");
            // If reached end of branch (can't go further) check if enough steps have been made in deepest nod, if they have place box and continue to next
            if (deepest_node.stepped.Count >= min_steps)
            {
                Debug.Log("BOX PLACED");
                room.grid[current_node.pos.x, current_node.pos.y] += (int)Elements.box;
                box_positions.Add(current_node.pos);
                BoxPlace(room);
            }
            else
            {
                if (current_node.stepped.Count > 1)
                {
                    Debug.Log("STEPPING BACK");
                    StartCoroutine(CheckNode(current_node.stepped[current_node.stepped.Count - 2], room));
                }
                else
                {
                    Debug.Log("RESTARTING");
                    room.grid = empty_grid;
                    PlaceButtons(room);
                }
            }
        }
    }

    void NewRoom(Room room)
    {
        StopAllCoroutines();
        GetComponent<GenerateGrid>().Restart(room);
    }

    void PlacePlayer(Direction dir, Room room)
    {
        Pos player_pos = new Pos();
        //Spawns the player next to the last placed box

        if (dir == Direction.N)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x;
            player_pos.y = box_positions[box_positions.Count - 1].y + 1;
        }
        if (dir == Direction.E)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x + 1;
            player_pos.y = box_positions[box_positions.Count - 1].y;
        }
        if (dir == Direction.S)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x;
            player_pos.y = box_positions[box_positions.Count - 1].y - 1;
        }
        if (dir == Direction.W)
        {
            player_pos.x = box_positions[box_positions.Count - 1].x - 1;
            player_pos.y = box_positions[box_positions.Count - 1].y;
        }

        room.grid[player_pos.x, player_pos.y] += (int)Elements.player;


        GetComponent<GenerateObjects>().Generate(room);
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
}
