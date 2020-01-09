using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class BoxMovement : MonoBehaviour
{
    public float move_speed = 2;
    private GameObject grid;
    GridSetup grid_scr;
    public int x_pos, y_pos = 0;
    Vector3 move_target;
    private bool moving = false;

    void Start()
    {
        grid = GameObject.FindWithTag("Grid");
        grid_scr = grid.GetComponent<GridSetup>();
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, move_target, Time.deltaTime * move_speed);
        }

        if (transform.position == move_target)
        {
            moving = false;
        }
    }

    public bool MoveBox(int x_dir, int y_dir)
    {
        if (grid_scr.CheckEdge(x_pos + x_dir, y_pos + y_dir))
        {
            if (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.childCount == 0 ||
                (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.childCount == 1 &&
                grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.GetChild(0).gameObject.tag == "Button"))
            {
                transform.parent = null;
                transform.parent = grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform;
                x_pos = x_pos + x_dir;
                y_pos = y_pos + y_dir;
                move_target = new Vector3(transform.parent.position.x, 0.6f, transform.parent.position.z);
                moving = true;
                return true;
            }
        }
        return false;
    }
}