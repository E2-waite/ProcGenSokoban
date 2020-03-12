using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Node : MonoBehaviour
{
    public Pos pos;
    public bool is_wall;
    public Node parent; // Stores the parent node, allows for looping from the end node back to the beginning node

    public int g_cost;
    public int h_cost;
    public int f_cost { get { return g_cost + h_cost; } }

    public Node (bool _is_wall, Pos _pos)
    {
        is_wall = _is_wall;
        pos = _pos;
    }
}
