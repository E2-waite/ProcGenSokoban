﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace enums
{
    public class Room
    {
        public Level parent_level;
        public Pos pos;
        public GameObject room_object;
        public Elements[,] grid;
        public GameObject[,] object_grid;
        public int offset_x, offset_y;
        public int num_templates, size_x, size_y, grid_x, grid_y, num_boxes;
        public bool solved, generated = false, first = false, last = false, instantiated = false;
        public Direction entrance_dir;
        public List<Direction> exit_dirs;
        public Pos entrance;
        public List<Pos> exits;
        public Stage stage = Stage.level;
        public List<GameObject> buttons = new List<GameObject>();
    }

    public class Attempt
    {
        public bool solved, failed;
    }


    public enum Stage
    { 
        level,
        grid,
        buttons,
        boxes,
        path,
        complete
    }


    public class Level
    {
        public GameObject level_object;
        public Elements[,] grid;
        public Room[,] room_grid;
        public GameObject[,] object_grid;
        public List<Cell> maze_cells = new List<Cell>();
        public bool generated = false;
        public bool instantiated = false;
        public GameObject player;
        public List<GameObject> boxes = new List<GameObject>();
        public List<Move> moves = new List<Move>();
    }

    public class Move
    {
        public Pos player_pos;
        public List<Pos> box_pos = new List<Pos>();
    }

    public struct IntVec2
    {
        public IntVec2(int in_x, int in_y)
        {
            x = in_x;
            y = in_y;
        }
        public int x;
        public int y;
    }

    public class Pos
    {
        public Pos()
        {

        }
        public Pos GetNewPos(Direction dir)
        {
            if (dir == Direction.N) return new Pos(x, y + 1);
            else if (dir == Direction.E) return new Pos(x + 1, y);
            else if (dir == Direction.S) return new Pos(x, y - 1);
            else if (dir == Direction.W) return new Pos(x - 1, y);
            return null;
        }

        public Pos(int x_pos, int y_pos)
        {
            x = x_pos;
            y = y_pos;
        }
        public int x;
        public int y;
        public bool empty = false;
    }

    public class Template
    {
        public int[,] template;
        public List<int>[] compatible = new List<int>[4];
    }

    public enum TILE : int 
    { 
        blank = 0,
        floor = 1,
        wall = 2,
        enterance = 3,
        exit = 4
    }

    public enum Direction
    {
        N,
        E,
        S,
        W,
        None,
    }
    
    public enum Elements : int
    {
        empty = 0,
        floor = 1,
        wall = 2,
        player = 4,
        box = 8,
        button = 16,
        entrance = 32,
        exit = 64, 
        trapdoor = 128,
        dead = 256
    }
}
