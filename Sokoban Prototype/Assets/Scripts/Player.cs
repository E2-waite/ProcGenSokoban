using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class Player : MonoBehaviour
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
        if (Input.GetKey("up") && !moving) MovePlayer(0, 1);
        if (Input.GetKey("left") && !moving) MovePlayer(-1, 0);
        if (Input.GetKey("down") && !moving) MovePlayer(0, -1);
        if (Input.GetKey("right") && !moving) MovePlayer(1, 0);
        if (Input.GetKeyUp("z") && !moving) game.StepBack();
        if (moving) transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * move_speed);
        if (transform.position == target) moving = false;
        
        // Game is won if player is stood on open trap door
        if (!moving && transform.parent != null && transform.parent.CompareTag("Trapdoor") && transform.parent.GetComponent<DoorAction>().IsOpen())
        {
            game.Win();
        }
    }

    private void MovePlayer(int x_dir, int y_dir)
    {
        int x_pos = (int)(transform.position.x + x_dir), y_pos = (int)(transform.position.z + y_dir);
        GameObject tile = game.this_level.object_grid[x_pos, y_pos];

        if (tile.CompareTag("Floor") || (tile.CompareTag("Doorway") && tile.GetComponent<DoorAction>().IsOpen()) || tile.CompareTag("Trapdoor"))
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
        target = new Vector3(transform.parent.position.x, 0.6f, transform.parent.position.z);
        moving = true;
        pos = new Pos() { x = (int)transform.parent.position.x, y = (int)transform.parent.position.y };
        game.UpdateMove();
    }
}
