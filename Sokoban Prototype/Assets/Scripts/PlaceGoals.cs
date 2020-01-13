using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;

public class PlaceGoals : MonoBehaviour
{
    public GameObject player_prefab, box_prefab, button_prefab;
    private GameObject[] boxes, buttons;
    private GameObject player;
    public int num_boxes = 2,  max_attempts = 10,  max_configs = 10, min_moves = 3;
    private int num_attempts = 0, num_configs = 0, boxes_checked = 0, moves_made = 0;
    int [] highest_moves;
    public void StartPlacing()
    {
        highest_moves = new int[num_boxes];
        StartCoroutine(PlaceObjects());
    }

    private void DiscardLayout()
    {
        StopAllCoroutines();
        num_attempts = 0; num_configs = 0; boxes_checked = 0;
        GetComponent<GenerateGrid>().Restart();
    }

    private void Update()
    {
        if (boxes_checked >= num_boxes)
        {
            boxes_checked = 0;
            StartCoroutine(CheckHighestMoves());
        }
    }

    private IEnumerator CheckHighestMoves()
    {
        bool above_threshold = false;
        for (int i = 0; i < num_boxes; i++)
        {
            if (highest_moves[i] > min_moves)
            {
                above_threshold = true;
            }
            else
            {
                break;
            }
            yield return null;
        }

        if (moves_made > min_moves && boxes.Length == num_boxes)
        {
            Debug.Log("ALL BOXES ABOVE MIN MOVES THRESHOLD, KEEPING LAYOUT");
            StopAllCoroutines();
        }
        else
        {
            moves_made = 0;
            if (num_configs < max_configs)
            {
                if (num_attempts < max_attempts)
                {
                    StartCoroutine(Reset());
                }
                else
                {
                    StartCoroutine(Refresh());
                }
            }
            else
            {
                Debug.Log("MAX CONFIGS REACHED, DISCARDING LAYOUT");
                DiscardLayout();           
            }
        }
    }

    private IEnumerator PlaceObjects()
    {
        GenerateGrid grid = GetComponent<GenerateGrid>();
        boxes = new GameObject[num_boxes];
        buttons = new GameObject[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            bool placed = false;        
            while (!placed)
            {
                int rand = Random.Range(0, grid.floor_tiles.Count);
                if (grid.floor_tiles[rand].transform.childCount == 0)
                {
                    buttons[i] = Instantiate(button_prefab, new Vector3(grid.floor_tiles[rand].transform.position.x, 0.5f, grid.floor_tiles[rand].transform.position.z), Quaternion.identity);
                    boxes[i] = Instantiate(box_prefab, new Vector3(grid.floor_tiles[rand].transform.position.x, 0.5f, grid.floor_tiles[rand].transform.position.z), Quaternion.identity);
                    buttons[i].transform.parent = grid.floor_tiles[rand].transform;
                    buttons[i].name = "Button " + (i + 1).ToString();
                    boxes[i].transform.parent = grid.floor_tiles[rand].transform;
                    boxes[i].name = "Box " + (i + 1).ToString();
                    placed = true;
                }
                yield return null;
            }                  
        }

        StartCoroutine(StartAttempt()); 
    }



    private IEnumerator Reset()
    {
        num_attempts++;
        highest_moves = new int[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            boxes[i].transform.position = buttons[i].transform.position;
            boxes[i].transform.parent = GetComponent<GenerateGrid>().GetTile((int)transform.position.x, (int)transform.position.z).transform;
            yield return null;
        }
        StartCoroutine(StartAttempt());
    }

    private IEnumerator Refresh()
    {
        num_configs++;
        highest_moves = new int[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            Destroy(buttons[i]);
            Destroy(boxes[i]);
            yield return null;
        }
 
        StartCoroutine(PlaceObjects());
    }

    private IEnumerator StartAttempt()
    {
        //Debug.Log("STARTING ATTEMPT");
        for (int i = 0; i < num_boxes; i++)
        {
            StartCoroutine(AttemptPlacement(i));
            yield return null;
        }  
    }

    private IEnumerator AttemptPlacement(int box_num)
    {
        if (boxes[box_num] != null)
        {
            int x = (int)boxes[box_num].transform.position.x, y = (int)boxes[box_num].transform.position.z;
            for (int i = 0; i < max_attempts; i++)
            {
                bool[] dir_checked = new bool[4];
                int num_moves = 0;
                List<GameObject> prev_tiles = new List<GameObject>();
                StartCoroutine(CheckDirection(x, y, 0, box_num, num_moves, dir_checked, prev_tiles));
                yield return null;
            }
        }
        else
        {
            DiscardLayout();
        }
    }

    private IEnumerator CheckDirection(int x, int y, int dir, int box_num, int num_moves, bool[] dir_checked, List<GameObject> prev_tiles)
    {
        int num_steps = 0, x_dir = 0, y_dir = 0;
        bool can_move = false, dir_selected = false;

        // Randomly select direction until unchecked direction is selected
        dir = Random.Range(0, 4);
        while (!dir_selected)
        {
            if (dir_checked[dir]) dir = Random.Range(0, 4);
            else dir_selected = true;
            yield return null;
        }

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
            num_steps++;
            x = x + x_dir;
            y = y + y_dir;
            can_move = CanMove(x, x_dir, y, y_dir);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveInDirection(x, y, dir, num_steps, box_num, num_moves, dir_checked, prev_tiles));
    }

    private IEnumerator MoveInDirection(int x, int y, int dir, int num_steps, int box_num, int num_moves, bool[] dir_checked, List<GameObject> prev_tiles)
    {
        // If the tile is free and box hasn't already been placed on tile, place the box on desired tile
        if (num_steps > 0 && !prev_tiles.Contains(GetComponent<GenerateGrid>().GetTile(x,y)) && 
            GetComponent<GenerateGrid>().GetTile(x, y).transform.childCount == 0)
        {
            // Reset directions checked and apply oposite to direction to prevent moving to previous space
            dir_checked = new bool[4];
            if (dir == (int)DIRECTION.up) dir_checked[(int)DIRECTION.down] = true;
            else if (dir == (int)DIRECTION.right) dir_checked[(int)DIRECTION.left] = true;
            else if (dir == (int)DIRECTION.down) dir_checked[(int)DIRECTION.up] = true;
            else if (dir == (int)DIRECTION.left) dir_checked[(int)DIRECTION.right] = true;

            // Move box to new position
            //Debug.Log((box_num + 1).ToString() + " Could Move in Direction: " + (DIRECTION)dir + " " + num_steps.ToString() + " Times");
            if (boxes[box_num] == null)
            {
                //StartCoroutine(Refresh());
                yield break;
            }
            else
            {
                boxes[box_num].transform.parent = GetComponent<GenerateGrid>().GetTile(x, y).transform;
                yield return new WaitForSeconds(0.1f);
                boxes[box_num].transform.position = boxes[box_num].transform.parent.position;
                prev_tiles.Add(boxes[box_num].transform.parent.gameObject);
                num_moves++;
                dir = Random.Range(0, 4);
                StartCoroutine(CheckDirection(x, y, dir, box_num, num_moves, dir_checked, prev_tiles));
            }
        }
        else
        {
            //Debug.Log((box_num + 1).ToString() + " Could Not Move in Direction: " + (DIRECTION)dir + "(" + num_moves.ToString() + " Moves Completed)");
            dir_checked[dir] = true;
            bool all_checked = false;
            for (int i = 0; i < 4; i++)
            {
                if (dir_checked[i])
                {
                    all_checked = true;
                }
                else
                {
                    all_checked = false;
                    break;
                }             
                yield return null;
            }

            if (all_checked)
            {
                boxes_checked++;
                moves_made += num_moves;
                //Debug.Log((box_num + 1).ToString() + " Could Not Move in Any Direction");               
            }
            else
            {
                // If all of the directions have not been checked, check another direction
                StartCoroutine(CheckDirection(x, y, dir, box_num, num_moves, dir_checked, prev_tiles));
                yield break;
            }
        }
    }

    private bool CanMove(int x, int x_dir, int y, int y_dir)
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
