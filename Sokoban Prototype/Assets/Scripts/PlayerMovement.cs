using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
        if (Input.GetKeyUp("up"))
        {
            MovePlayer(0, 1);
        }
        if (Input.GetKeyUp("left"))
        {
            MovePlayer(-1, 0);
        }
        if (Input.GetKeyUp("down"))
        {
            MovePlayer(0, -1);
        }
        if (Input.GetKeyUp("right"))
        {
            MovePlayer(1, 0);
        }
        if (Input.GetKeyUp("r"))
        {
            grid_scr.ResetGame();
        }
        if (Input.GetKeyUp("t"))
        {
            grid_scr.RefreshGame();
        }

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, move_target, Time.deltaTime * move_speed);
        }
        if (transform.position == move_target)
        {
            moving = false;
        }
    }

    void MovePlayer(int x_dir, int y_dir)
    {
        if (grid_scr.CheckEdge(x_pos + x_dir, y_pos + y_dir) && !moving)
        {
            if (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.childCount == 0)
            {
                // Move if there is nothing on the desired tile;
                Movement(x_dir, y_dir);
            }
            else
            {
                if (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.GetChild(0).gameObject.tag == "Box")
                {
                    // If a box is on the desired tile, move player only if box moves
                    BoxMovement box_scr = grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.GetChild(0).gameObject.GetComponent<BoxMovement>();
                    if (box_scr.MoveBox(x_dir, y_dir))
                    {
                        Movement(x_dir, y_dir);
                    }
                }
                else if (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.GetChild(0).gameObject.tag == "Button")
                {
                    if (grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.childCount == 1)
                    {
                        Movement(x_dir, y_dir);
                    }
                    else
                    {
                        BoxMovement box_scr = grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform.GetChild(1).gameObject.GetComponent<BoxMovement>();
                        if (box_scr.MoveBox(x_dir, y_dir))
                        {
                            Movement(x_dir, y_dir);
                        }
                    }
                }
            }            
        }
    }

    void Movement(int x_dir, int y_dir)
    {
        transform.parent = null;
        transform.parent = grid_scr.GetTile(x_pos + x_dir, y_pos + y_dir).transform;
        x_pos = x_pos + x_dir;
        y_pos = y_pos + y_dir;
        move_target = new Vector3(transform.parent.position.x, 0.5f, transform.parent.position.z);
        moving = true;
    }
}
