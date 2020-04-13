using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Box : MonoBehaviour
{
    public Pos pos;
    public float move_speed = 5;
    private GameControl game;
    private Vector3 target;
    private bool moving = false;

    private void Start()
    {
        game = GameObject.FindWithTag("Grid").GetComponent<GameControl>();
    }

    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * move_speed);
        }

        if (transform.position == target)
        {
            moving = false;
        }
    }

    public bool Move(int x_dir, int y_dir)
    {
        int x_pos = (int)transform.parent.position.x + x_dir, y_pos = (int)transform.parent.position.z + y_dir;
        GameObject tile = game.this_level.object_grid[x_pos, y_pos];
        if (tile.CompareTag("Floor") &&
            (tile.transform.childCount == 0 ||
            (tile.transform.childCount == 1 &&
            tile.transform.GetChild(0).CompareTag("Button"))) ||
            tile.CompareTag("Trapdoor") ||
            (tile.CompareTag("Doorway") && tile.GetComponent<DoorAction>().IsOpen()))
        {
            transform.parent = null;
            transform.parent = tile.transform;
            target = new Vector3(transform.parent.position.x, 0.5f, transform.parent.position.z);
            moving = true;
            pos = new Pos() { x = (int)transform.parent.position.x, y = (int)transform.parent.position.y };
            return true;
        }
        return false;
    }
}
