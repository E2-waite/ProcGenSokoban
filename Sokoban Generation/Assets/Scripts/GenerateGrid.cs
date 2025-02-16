﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GenerateGrid
{
    public void Generate(Cell cell, Room room)
    {
        room.generated = false;
        room.entrance_dir = cell.entrance;
        room.exit_dirs = cell.exits;
        bool finished = false;
        while (!finished)
        {
            if (CombineTemplates(room))
            {
                finished = true;
            }
        }
    }

    Direction GetDoorwayDir(Direction dir, Room room)
    {
        if (room.entrance_dir == dir)
        {
            return dir;
        }
        for (int i = 0; i < room.exit_dirs.Count; i++)
        {
            if (room.exit_dirs[i] == dir)
            {
                return dir;
            }
        }
        return Direction.None;
    }

    private bool CombineTemplates(Room room)
    {
        Templates templates = new Templates();
        room.grid = new Elements[room.grid_x, room.grid_y];
        Elements[,] temp_grid = new Elements[room.grid_x, room.grid_y];
        int i = 0, x_pos = 0, y_pos = 0;
        while (i < room.num_templates)
        {
            temp_grid = room.grid;

            // Gets compatible template for the required direction of door
            Direction doorway_dir = Direction.None;
            if (y_pos == room.size_y - 1 && x_pos == Mathf.RoundToInt(room.size_x / 2))
            {
                doorway_dir = GetDoorwayDir(Direction.N, room);
            }
            if (y_pos == Mathf.RoundToInt(room.size_y / 2) && x_pos == room.size_x - 1)
            {
                doorway_dir = GetDoorwayDir(Direction.E, room);
            }
            if (y_pos == 0 && x_pos == Mathf.RoundToInt(room.size_x / 2))
            {
                doorway_dir = GetDoorwayDir(Direction.S, room);
            }
            if (y_pos == Mathf.RoundToInt(room.size_y / 2) && x_pos == 0)
            {
                doorway_dir = GetDoorwayDir(Direction.W, room);
            }

            // Gets random template from list of templates
            int[,] template = templates.GetTemplate(doorway_dir).template;

            // Start applying template
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    //Debug.Log("X:" + (x + x_pos).ToString() + " Y:" + (y + y_pos).ToString());
                    if (temp_grid[x + (x_pos * 3), y + (y_pos * 3)] == 0 || temp_grid[x + (x_pos * 3), y + (y_pos * 3)] == (Elements)template[x, y])
                    {
                        temp_grid[x + (x_pos * 3), y + (y_pos * 3)] = (Elements)template[x, y];
                    }
                }
            }

            // Go to next position
            room.grid = temp_grid;
            i++;
            if (IsMultipleOf(i, room.size_x))
            {
                x_pos = 0;
                y_pos++;
            }
            else
            {
                x_pos++;
            }
        }

        // Place walls around the generated room
        for (int y = 0; y < room.grid_y; y++)
        {
            for (int x = 0; x < room.grid_x; x++)
            {
                if (x == 0 || x == room.grid_x - 1 || y == 0 || y == room.grid_y - 1)
                {
                    room.grid[x, y] = Elements.wall;
                }
            }
        }

        return CheckGrid(room);
    }

    private bool IsMultipleOf(int x, int n)
    {
        // Returns the remainder after dividing x by n which will always be 0 if x is divisible by n.
        return (x % n) == 0;
    }

    private bool CheckGrid(Room room)
    {
        GridCheck check = new GridCheck();
        int num_floors = check.FloorCount(room.grid);
        if (num_floors < (room.grid.GetLength(0) * room.grid.GetLength(1)) / 4 || !check.ContinuousFloor(room.grid, num_floors))
        {
            return false;
        }
        return PlaceDoorways(room);
    }


    bool PlaceDoorways(Room room)
    {
        bool generate = true;
        // Do not place entrance doorway in first room
        room.exits = new List<Pos>();
        for (int i = 0; i < room.exit_dirs.Count; i++)
        {
            if (!PlaceDoorway(room.exit_dirs[i], Elements.exit, room))
            {
                generate = false;
                break;
            }
        }

        if (room.first)
        {
            room.entrance = new Pos { x = Mathf.RoundToInt(room.grid.GetLength(0) / 2), y = Mathf.RoundToInt(room.grid.GetLength(1) / 2) };
        }
        if (room.last)
        {
            room.exits.Add(new Pos { x = Mathf.RoundToInt(room.grid.GetLength(0) / 2), y = Mathf.RoundToInt(room.grid.GetLength(1) / 2) });
        }

        if (room.last)
        {
            if (!room.first && !PlaceDoorway(room.entrance_dir, Elements.entrance, room))
            {
                generate = false;
            }
            if (!CheckCentre(room))
            {
                generate = false;

            }
        }
        else if (room.first)
        {
            if (!CheckCentre(room))
            {
                generate = false;
            }
        }
        else
        {
            if (!PlaceDoorway(room.entrance_dir, Elements.entrance, room))
            {
                generate = false;
            }
        }

        if (generate)
        {
            if (room.last)
            {
                room.grid[room.exits[0].x, room.exits[0].y] = Elements.trapdoor;
            }
            return new GeneratePuzzle().Generate(room);
        }
        else
        {
            return false;
        }
    }

    bool CheckCentre(Room room)
    {
        if (room.grid[Mathf.RoundToInt(room.grid.GetLength(0) / 2), Mathf.RoundToInt(room.grid.GetLength(1) / 2)] == Elements.wall)
        {
            return false;
        }
        return true;
    }

    bool PlaceDoorway(Direction edge, Elements type, Room room)
    {
        int x = 0, y = 0;
        switch (edge)
        {
            case Direction.N:
                {
                    x = (int)(room.grid.GetLength(0) / 2); 
                    y = room.grid.GetLength(1) - 1;
                    room.grid[x, y] = type;
                    if (room.grid[x, y - 1] == Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.E:
                {
                    x = room.grid.GetLength(0) - 1;
                    y = (int)(room.grid.GetLength(1) / 2);
                    room.grid[x, y] = type;
                    if (room.grid[x - 1, y] == Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.S:
                {
                    x = (int)(room.grid.GetLength(0) / 2); 
                    y = 0;
                    room.grid[x, y] = type;
                    if (room.grid[x, y + 1] == Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
            case Direction.W:
                {
                    x = 0;
                    y = (int)(room.grid.GetLength(1) / 2);
                    room.grid[x, y] = type;
                    if (room.grid[x + 1, y] == Elements.wall)
                    {
                        return false;
                    }
                    break;
                }
        }

        if (type == Elements.entrance)
        {
            room.entrance = new Pos { x = x, y = y };
        }
        else if (type == Elements.exit)
        {
            room.exits.Add( new Pos { x = x, y = y } );
        }
        return true;
    }
}
