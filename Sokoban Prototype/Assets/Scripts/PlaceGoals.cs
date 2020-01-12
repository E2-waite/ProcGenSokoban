using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;

public class PlaceGoals : MonoBehaviour
{
    public GameObject player_prefab, box_prefab, button_prefab;
    private GameObject[] boxes, buttons;
    private GameObject player;
    public int num_boxes = 2, max_attempts = 10, max_configs = 10, min_moves = 3;
    
    public void StartPlacing()
    {
        StartCoroutine(PlaceObjects());
    }

    void Restart()
    {

    }

    IEnumerator PlaceObjects()
    {
        GenerateGrid grid = GetComponent<GenerateGrid>();
        boxes = new GameObject[num_boxes];
        buttons = new GameObject[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            bool placed = false;
            int rand = Random.Range(0, grid.floor_tiles.Count);
            while (!placed)
            {
                if (grid.floor_tiles[rand].transform.childCount == 0)
                {
                    buttons[i] = Instantiate(button_prefab, new Vector3(grid.floor_tiles[rand].transform.position.x, 0.5f, grid.floor_tiles[rand].transform.position.z), Quaternion.identity);
                    boxes[i] = Instantiate(box_prefab, new Vector3(grid.floor_tiles[rand].transform.position.x, 0.5f, grid.floor_tiles[rand].transform.position.z), Quaternion.identity);
                    buttons[i].transform.parent = grid.floor_tiles[rand].transform;
                    boxes[i].transform.parent = grid.floor_tiles[rand].transform;
                    boxes[i].name = "Box " + (i + 1).ToString();
                    placed = true;
                }
                yield return null;
            }                  
        }
        for (int i = 0; i < max_configs; i++)
        {
            StartCoroutine(StartAttempt());
            yield return null;
        }     
    }

    IEnumerator StartAttempt()
    {
        for (int i = 0; i < num_boxes; i++)
        {
            StartCoroutine(AttemptPlacement(i));
            yield return null;
        }  
    }

    IEnumerator AttemptPlacement(int box_num)
    {
        int x = (int)boxes[box_num].transform.position.x, y = (int)boxes[box_num].transform.position.z;
        for (int i = 0; i < max_attempts; i++)
        {
            int dir = Random.Range(0, 4);
            StartCoroutine(CheckDirection(x,y,dir,box_num));          
            yield return null;
        }
    }

    IEnumerator CheckDirection(int x, int y, int dir, int num)
    {
        int num_moves = 0, x_dir = 0, y_dir = 0;
        bool can_move = false;

        if ((DIRECTION)dir == DIRECTION.up)
        {
            x_dir = 0;
            y_dir = -1;
        }
        if ((DIRECTION)dir == DIRECTION.right)
        {
            x_dir = 1;
            y_dir = 0;
        }
        if ((DIRECTION)dir == DIRECTION.down)
        {
            x_dir = 0;
            y_dir = 1;
        }
        if ((DIRECTION)dir == DIRECTION.left)
        {
            x_dir = -1;
            y_dir = 0;
        }

        can_move = CanMove(x, x_dir, y, y_dir);
        while (can_move)
        {
            Debug.Log("CAN MOVE");
            num_moves++;
            x = x + x_dir;
            y = y + y_dir;
            can_move = CanMove(x, x_dir, y, y_dir);
            yield return null;
        }

        StartCoroutine(MoveInDirection(x, y, dir, num_moves, num));
    }

    IEnumerator MoveInDirection(int x, int y, int dir, int num_moves, int box_num)
    {
        if (num_moves > 0)
        {
            Debug.Log((box_num + 1).ToString() + " Could Move in Direction: " + (DIRECTION)dir + " " + num_moves.ToString() + " Times");
            yield return new WaitForSeconds(0.2f);
            boxes[box_num].transform.position = new Vector3(x, 0.5f, y);
        }
        else Debug.Log((box_num + 1).ToString() + " Could Not Move in Direction: " + (DIRECTION)dir);     
    }

    bool CanMove(int x, int x_dir, int y, int y_dir)
    {
        if (GetComponent<GenerateGrid>().GetTile(x + x_dir, y + y_dir).tag == "Floor" &&
            GetComponent<GenerateGrid>().GetTile(x + (x_dir * 2), y + (y_dir * 2)).tag == "Floor" &&
            GetComponent<GenerateGrid>().GetTile(x + x_dir, y + y_dir).transform.childCount == 0 &&
            GetComponent<GenerateGrid>().GetTile(x + (x_dir * 2), y + (y_dir * 2)).transform.childCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
