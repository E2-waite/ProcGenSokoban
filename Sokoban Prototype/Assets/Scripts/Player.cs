using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Player : MonoBehaviour
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
        if (Input.GetKeyUp("up") && !moving) MovePlayer(0, 1);
        if (Input.GetKeyUp("left") && !moving) MovePlayer(-1, 0);
        if (Input.GetKeyUp("down") && !moving) MovePlayer(0, -1);
        if (Input.GetKeyUp("right") && !moving) MovePlayer(1, 0);
        if (moving) transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * move_speed);
        if (transform.position == target) moving = false;
    }

    private void MovePlayer(int x_dir, int y_dir)
    {
        int x_pos = (int)(transform.position.x + x_dir), y_pos = (int)(transform.position.z + y_dir);
        GameObject tile = game.this_level.object_grid[x_pos, y_pos];

        if (tile.CompareTag("Floor") || (tile.CompareTag("Doorway") && tile.GetComponent<DoorAction>().IsOpen()))
        {
            if (tile.transform.childCount == 0) Move(tile);
            else
            {
                if (tile.transform.GetChild(0).CompareTag("Box"))
                {
                    Box box = tile.transform.GetChild(0).GetComponent<Box>();
                    if (box.Move(x_dir, y_dir)) Move(tile);
                }
                else if (tile.transform.GetChild(0).CompareTag("Button"))
                {
                    if (tile.transform.childCount == 1) Move(tile);
                    else
                    {
                        Box box = tile.transform.GetChild(1).GetComponent<Box>();
                        if (box.Move(x_dir, y_dir)) Move(tile);
                    }
                }
            }
        }
    }

    private void Move(GameObject tile)
    {
        transform.parent = null;
        transform.parent = tile.transform;
        game.UpdatePosition(Elements.player, new int[2] { (int)transform.position.x, (int)transform.position.z },
            new int[2] { (int)transform.parent.position.x, (int)transform.parent.position.z });
        target = new Vector3(transform.parent.position.x, 0.6f, transform.parent.position.z);
        moving = true;
    }
}
