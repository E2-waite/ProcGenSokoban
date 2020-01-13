using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;

public class PlaceGoals : MonoBehaviour
{
    public GameObject player_prefab, box_prefab, button_prefab;
    private GameObject[] boxes, buttons;
    private GameObject player;
    private Vector2[] box_pos;
    private Vector2 player_pos;
    public int num_boxes = 2, max_attempts = 10, max_configs = 32, min_moves = 3;
    private int num_attempts = 0, num_configs = 0;
    int[] highest_moves;
    bool[] completed;
    public bool all_complete = false;
    DIRECTION last_dir;
    public void StartPlacing()
    {
        completed = new bool[num_boxes];
        highest_moves = new int[num_boxes];
        StartCoroutine(CheckComplete());
        StartCoroutine(PlaceObjects());
    }

    private void DiscardLayout()
    {
        StopAllCoroutines();
        num_attempts = 0; num_configs = 0;
        GetComponent<GenerateGrid>().Restart();
    }

    private void Update()
    {
        if (Input.GetKeyUp("r")) StartCoroutine(Reload());

        if (all_complete)
        {
            completed = new bool[num_boxes];
            all_complete = false;
            StartCoroutine(CheckHighestMoves());
        }
    }

    IEnumerator CheckComplete()
    {
        // Checks if all boxes have tried, and failed to move in all directions
        bool complete = false;
        for (int i = 0; i < num_boxes; i++)
        {
            if (completed[i]) complete = true;
            else
            {
                complete = false;
                break;
            }
            yield return null;
        }

        if (complete) all_complete = true;
        else
        {
            all_complete = false;
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(CheckComplete());
        }
    }

    private IEnumerator CheckHighestMoves()
    {
        bool above_threshold = false;
        for (int i = 0; i < num_boxes; i++)
        {
            if (highest_moves[i] >= min_moves)
            {
                above_threshold = true;
            }
            else
            {
                break;
            }
            yield return null;
        }

        if (above_threshold)
        {
            StopAllCoroutines();
            StartCoroutine(FinalCheck());
        }
        else
        {
            NextAttempt();
        }
    }

    void NextAttempt()
    {
        StopAllCoroutines();
        StartCoroutine(CheckComplete());
        if (num_configs < max_configs)
        {
            StartCoroutine(Refresh());
        }
        else
        {
            DiscardLayout();
        }
    }

    IEnumerator FinalCheck()
    {
        // Keeps looping until wall is reached, if a button is in line with a box the level is discarded
        for (int i = 0; i < num_boxes; i++)
        {
            boxes[i].transform.position = new Vector3(boxes[i].transform.parent.position.x, 0.5f, boxes[i].transform.parent.position.z);
            for (int j = 0; j < 4; j++)
            {
                int x = (int)boxes[i].transform.position.x, y = (int)boxes[i].transform.position.z;
                bool at_edge = false;
                while (!at_edge)
                {
                    if (j == 0) y++;
                    else if (j == 1) x++;
                    else if (j == 2) y--;
                    else if (j == 3) x--;

                    if (GetComponent<GenerateGrid>().GetTile(x, y).CompareTag("Wall"))
                    {
                        // If it reaches wall, begin checking next direction
                        break;
                    }

                    if (GetComponent<GenerateGrid>().GetTile(x, y).transform.childCount > 0 &&
                        GetComponent<GenerateGrid>().GetTile(x, y).transform.GetChild(0).CompareTag("Button"))
                    {
                        NextAttempt();
                        yield break;
                    }
                    yield return null;
                }
            }
        }

        StartCoroutine(PlacePlayer());
    }

    IEnumerator PlacePlayer()
    {
        int x = (int)boxes[num_boxes - 1].transform.position.x, y = (int)boxes[num_boxes - 1].transform.position.z;

        if (last_dir == DIRECTION.up) y--;
        else if (last_dir == DIRECTION.right) x--;
        else if (last_dir == DIRECTION.down) y++;
        else if (last_dir == DIRECTION.left) x++;

        if (GetComponent<GenerateGrid>().GetTile(x, y).transform.childCount == 0)
        {
            player = Instantiate(player_prefab, GetComponent<GenerateGrid>().GetTile(x, y).transform);
        }
        else
        {
            NextAttempt();
            yield break;
        }

        StartCoroutine(FurthestPlayerState(x, y));
    }

    IEnumerator FurthestPlayerState(int x, int y)
    {
        int highest_moves = 0;
        GameObject final_tile = null;
        for (int i = 0; i < 10; i++)
        {
            List<GameObject> checked_tiles = new List<GameObject>();
            bool[] dir_checked = new bool[4];
            int x_pos = x, y_pos = y, num_moves = 0;
            for (int j = 0; j < 10; j++)
            {
                bool all_checked = false;
                for (int k = 0; k < 4; k++)
                {
                    if (dir_checked[k])
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
                    break;
                }
                else
                {
                    DIRECTION dir = (DIRECTION)Random.Range(0, 4);
                    GameObject tile = GetTile(x_pos, y_pos, dir);
                    if (CheckTile(tile, checked_tiles))
                    {
                        num_moves++;
                        if (num_moves > 1 && num_moves > highest_moves)
                        {
                            highest_moves = num_moves;
                            final_tile = tile;
                        }
                        checked_tiles.Add(tile);
                        x_pos = (int)tile.transform.position.x;
                        y_pos = (int)tile.transform.position.z;

                        dir_checked = new bool[4];
                        if (dir == DIRECTION.up) dir_checked[(int)DIRECTION.down] = true;
                        else if (dir == DIRECTION.right) dir_checked[(int)DIRECTION.left] = true;
                        else if (dir == DIRECTION.down) dir_checked[(int)DIRECTION.up] = true;
                        else if (dir == DIRECTION.left) dir_checked[(int)DIRECTION.right] = true;
                    }
                    else
                    {
                        dir_checked[(int)dir] = true;
                    }
                }
                yield return null;
            }
        }

        if (final_tile != null)
        {
            player.transform.parent = final_tile.transform;
            player.transform.position = new Vector3(player.transform.parent.position.x, 0.5f, player.transform.parent.position.z);
            box_pos = new Vector2[num_boxes];
            for (int i = 0; i < num_boxes; i++)
            {
                box_pos[i] = new Vector2(boxes[i].transform.position.x, boxes[i].transform.position.z);
            }
            player_pos = new Vector2(player.transform.position.x, player.transform.position.z);
            GetComponent<CheckWin>().BeginChecking();
        }
        else
        {
            NextAttempt();
        }
    }

    GameObject GetTile(int x, int y, DIRECTION dir)
    {
        if (dir == DIRECTION.up) y++;
        if (dir == DIRECTION.right) x++;
        if (dir == DIRECTION.down) y--;
        if (dir == DIRECTION.left) x--;
        return GetComponent<GenerateGrid>().GetTile(x, y);
    }

    bool CheckTile(GameObject tile, List<GameObject>checked_tiles)
    {
        if (tile.transform.childCount == 0 && tile.CompareTag("Floor") && !checked_tiles.Contains(tile)) return true;
        else return false;
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

    public IEnumerator Reload()
    {
        GenerateGrid grid = GetComponent<GenerateGrid>();
        player.transform.parent = grid.GetTile((int)player_pos.x, (int)player_pos.y).transform;
        player.transform.position = new Vector3(player.transform.parent.position.x, 0.5f, player.transform.parent.position.z);
        for (int i = 0; i < num_boxes; i++)
        {
            boxes[i].transform.parent = grid.GetTile((int)box_pos[i].x, (int)box_pos[i].y).transform;
            boxes[i].transform.position = new Vector3(boxes[i].transform.parent.position.x, 0.5f, boxes[i].transform.parent.position.z);
            yield return null;
        }
    }

    private IEnumerator Restart()
    {
        // Starts new attempt to find furthest state from generated configuration
        num_attempts++;
        highest_moves = new int[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            boxes[i].transform.parent = GetComponent<GenerateGrid>().GetTile((int)transform.position.x, (int)transform.position.z).transform;
            yield return null;
        }
        StartCoroutine(StartAttempt());
    }

    private IEnumerator Refresh()
    {
        // Begins generation of new button position configuration
        num_attempts = 0;
        num_configs++;
        highest_moves = new int[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            Destroy(buttons[i]);
            Destroy(boxes[i]);
            Destroy(player);
            yield return null;
        }
 
        StartCoroutine(PlaceObjects());
    }

    private IEnumerator StartAttempt()
    {
        //for (int i = 0; i < max_attempts; i++)
        //{
            for (int j = 0; j < num_boxes; j++)
            {
                StartCoroutine(AttemptPlacement(j));
                yield return null;
            }
        //}
    }

    private IEnumerator AttemptPlacement(int box_num)
    {
        int x = (int)boxes[box_num].transform.position.x, y = (int)boxes[box_num].transform.position.z;
        bool[] dir_checked = new bool[4];
        int num_moves = 0;
        List<GameObject> prev_tiles = new List<GameObject>();
        StartCoroutine(CheckDirection(x, y, box_num, num_moves, dir_checked, prev_tiles));

        yield return null;
    }

    private IEnumerator CheckDirection(int x, int y, int box_num, int num_moves, bool[] dir_checked, List<GameObject> prev_tiles)
    {
        
        int num_steps = 0, x_dir = 0, y_dir = 0;
        bool can_move, dir_selected = false;

        // Randomly select direction until unchecked direction is selected
        int dir = Random.Range(0, 4);
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
            x += x_dir;
            y += y_dir;
            can_move = CanMove(x, x_dir, y, y_dir);
            yield return null;
        }

        StartCoroutine(MoveInDirection(x, y, dir, num_steps, box_num, num_moves, dir_checked, prev_tiles));
    }

    private IEnumerator MoveInDirection(int x, int y, int dir, int num_steps, int box_num, int num_moves, bool[] dir_checked, List<GameObject> prev_tiles)
    {
        // If the tile is free and box hasn't already been placed on tile, place the box on desired tile
        if (num_steps > 0 && !prev_tiles.Contains(GetComponent<GenerateGrid>().GetTile(x,y)) && 
            GetComponent<GenerateGrid>().GetTile(x, y).transform.childCount == 0 &&
            GetComponent<GenerateGrid>().GetTile(x, y).CompareTag("Floor"))
        {
            // Reset directions checked and apply oposite to direction to prevent moving to previous space
            dir_checked = new bool[4];
            if (dir == (int)DIRECTION.up) dir_checked[(int)DIRECTION.down] = true;
            else if (dir == (int)DIRECTION.right) dir_checked[(int)DIRECTION.left] = true;
            else if (dir == (int)DIRECTION.down) dir_checked[(int)DIRECTION.up] = true;
            else if (dir == (int)DIRECTION.left) dir_checked[(int)DIRECTION.right] = true;

            // Move box to new position
            if (boxes[box_num] == null)
            {
                yield break;
            }
            else
            {
                boxes[box_num].transform.parent = GetComponent<GenerateGrid>().GetTile(x, y).transform;
                prev_tiles.Add(boxes[box_num].transform.parent.gameObject);
                highest_moves[box_num]++;
                dir = Random.Range(0, 4);
                last_dir = (DIRECTION)dir;
                StartCoroutine(CheckDirection(x, y, box_num, num_moves, dir_checked, prev_tiles));
            }
        }
        else
        {
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
                completed[box_num] = true;
                yield break;
            }
            else
            {
                // If all of the directions have not been checked, check another direction
                StartCoroutine(CheckDirection(x, y, box_num, num_moves, dir_checked, prev_tiles));
                yield break;
            }
        }
    }

    private bool CanMove(int x, int x_dir, int y, int y_dir)
    {
        if (GetComponent<GenerateGrid>().GetTile(x + x_dir, y + y_dir).CompareTag("Floor") &&
            GetComponent<GenerateGrid>().GetTile(x + (x_dir * 2), y + (y_dir * 2)).CompareTag("Floor") &&
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

    public List<GameObject> GetButtons()
    {
        List<GameObject> button_list = new List<GameObject>();

        foreach (GameObject obj in buttons)
        {
            button_list.Add(obj);
        }

        return button_list;
    }
}
