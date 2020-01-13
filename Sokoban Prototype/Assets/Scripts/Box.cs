using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public float move_speed = 5;
    private GenerateGrid grid;
    private Vector3 target;
    private bool moving = false;
    
    private void Start()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<GenerateGrid>();
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
        GameObject tile = grid.GetTile(x_pos, y_pos);
        if (tile.CompareTag("Floor") && 
            (tile.transform.childCount == 0 ||
            (tile.transform.childCount == 1 &&
            tile.transform.GetChild(0).CompareTag("Button"))))
        {
            transform.parent = null;
            transform.parent = tile.transform;
            target = new Vector3(transform.parent.position.x, 0.6f, transform.parent.position.z);
            moving = true;
            return true;
        }
        return false;
    }
}
