using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Box : MonoBehaviour
{
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
        GameObject tile = game.object_grid[x_pos, y_pos];
        if (tile.CompareTag("Floor") &&
            (tile.transform.childCount == 0 ||
            (tile.transform.childCount == 1 &&
            tile.transform.GetChild(0).CompareTag("Button"))))
        {
            transform.parent = null;
            transform.parent = tile.transform;
            game.UpdatePosition(Elements.box, new int[2] { (int)transform.position.x, (int)transform.position.z },
            new int[2] { (int)transform.parent.position.x, (int)transform.parent.position.z });
            target = new Vector3(transform.parent.position.x, 0.5f, transform.parent.position.z);
            moving = true;
            return true;
        }
        return false;
    }
}
