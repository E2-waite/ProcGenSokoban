using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace enums
{
    public class Room
    {
        public GameObject room_object;
        public int[,] grid;
        public GameObject[,] object_grid;
        public int offset_x, offset_y;
        public int num_templates, size_x = 3, size_y = 3, grid_x = 0, grid_y = 0;
    }

    public class Pos
    {
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
        None
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
        exit = 64
    }
}
