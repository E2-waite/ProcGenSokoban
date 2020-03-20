using System.Collections;
using System.Collections.Generic;
namespace enums
{
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
