using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class RenderWall : MonoBehaviour
{
    public GameObject NorthWall, EastWall, SouthWall, WestWall;
    public void EnableWalls(bool[] walls)
    {
        if (walls[(int)Direction.N]) NorthWall.SetActive(true);
        if (walls[(int)Direction.E]) EastWall.SetActive(true);
        if (walls[(int)Direction.S]) SouthWall.SetActive(true);
        if (walls[(int)Direction.W]) WestWall.SetActive(true);
    }
}
