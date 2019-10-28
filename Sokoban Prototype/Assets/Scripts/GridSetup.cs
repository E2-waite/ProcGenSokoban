using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetup : MonoBehaviour
{
    public int size_x = 4, size_y = 4;
    private int num_boxes = 0;
    private bool set_up = false;
    public GameObject tile_prefab;
    public GameObject player_prefab;
    public GameObject box_prefab;
    public GameObject button_prefab;
    private GameObject[,] tiles;
    private GameObject player;
    private GameObject[] boxes;
    private GameObject[] buttons;
    private Vector3 pl_pos;
    private Vector3[] box_pos;
    private Vector3[] btn_pos;
    public bool game_won = false;
    void Start()
    {
        // Setup grid tiles
        tiles = new GameObject[size_x, size_y];
        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                tiles[j, i] = Instantiate(tile_prefab, new Vector3(j, 0, i), Quaternion.identity);
                tiles[j, i].transform.parent = this.transform;
                tiles[j, i].name = "Tile " + i.ToString() + "," + j.ToString();
            }
        }

        RefreshGame();
    }

    void Update()
    {
        if (set_up)
        {
            if (CheckWin())
            {
                game_won = true;
            }
        }
    }

    bool CheckWin()
    {
        // Checks if all buttons have boxes on them
        for (int i = 0; i < num_boxes; i++)
        {
            if (buttons[i].transform.parent.childCount == 1)
            {
                return false;
            }
            if(buttons[i].transform.parent.GetChild(1).gameObject.tag == "Player")
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckEdge(int x, int y)
    {
        if (x < size_x && x >= 0 && y < size_y && y >= 0)
        {
            return true;
        }
        return false;
    }

    public GameObject GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public void ResetGame()
    {
        // Resets player and box to their original positions
        game_won = false;
        set_up = false;
        player.transform.position = pl_pos;
        player.transform.parent = tiles[Mathf.RoundToInt(pl_pos.x), Mathf.RoundToInt(pl_pos.z)].transform;
        PlayerMovement player_scr = player.GetComponent<PlayerMovement>();
        player_scr.x_pos = Mathf.RoundToInt(pl_pos.x);
        player_scr.y_pos = Mathf.RoundToInt(pl_pos.z);

        for (int i = 0; i < num_boxes; i++)
        {
            boxes[i].transform.parent = tiles[Mathf.RoundToInt(box_pos[i].x), Mathf.RoundToInt(box_pos[i].z)].transform;
            boxes[i].transform.position = box_pos[i];
            BoxMovement box_scr = boxes[i].GetComponent<BoxMovement>();
            box_scr.x_pos = Mathf.RoundToInt(box_pos[i].x);
            box_scr.y_pos = Mathf.RoundToInt(box_pos[i].z);
        }
        set_up = true;
    }

    public void RefreshGame()
    {
        // Resets game with new number of boxes and positions
        game_won = false;
        set_up = false;
        if (player != null)
        {
            Destroy(player);
        }
        if (num_boxes > 0)
        {
            for (int i = 0; i < num_boxes; i++)
            {
                Destroy(boxes[i]);
                Destroy(buttons[i]);                
            }
        }

        // Setup player in a random position
        pl_pos = new Vector3(Random.Range(0, size_x), 0.5f, Random.Range(0, size_y));
        player = Instantiate(player_prefab, pl_pos, Quaternion.identity);
        player.transform.parent = tiles[Mathf.RoundToInt(pl_pos.x), Mathf.RoundToInt(pl_pos.z)].transform;
        PlayerMovement player_scr = player.GetComponent<PlayerMovement>();
        player_scr.x_pos = Mathf.RoundToInt(pl_pos.x);
        player_scr.y_pos = Mathf.RoundToInt(pl_pos.z);

        // Setup boxes in random positions;
        num_boxes = Random.Range(2, 5);
        boxes = new GameObject[num_boxes];
        box_pos = new Vector3[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            bool box_placed = false;
            while (!box_placed)
            {
                box_pos[i] = new Vector3(Random.Range(1, size_x - 1), 0.6f, Random.Range(1, size_y - 1));
                if (tiles[Mathf.RoundToInt(box_pos[i].x), Mathf.RoundToInt(box_pos[i].z)].transform.childCount == 0)
                {
                    boxes[i] = Instantiate(box_prefab, box_pos[i], Quaternion.identity);
                    boxes[i].transform.parent = tiles[Mathf.RoundToInt(box_pos[i].x), Mathf.RoundToInt(box_pos[i].z)].transform;
                    BoxMovement box_scr = boxes[i].GetComponent<BoxMovement>();
                    box_scr.x_pos = Mathf.RoundToInt(box_pos[i].x);
                    box_scr.y_pos = Mathf.RoundToInt(box_pos[i].z);
                    box_placed = true;
                }
            }
        }

        buttons = new GameObject[num_boxes];
        btn_pos = new Vector3[num_boxes];
        for (int i = 0; i < num_boxes; i++)
        {
            bool btn_placed = false;
            while (!btn_placed)
            {
                btn_pos[i] = new Vector3(Random.Range(0, size_x), 0.15f, Random.Range(0, size_y));
                if (tiles[Mathf.RoundToInt(btn_pos[i].x), Mathf.RoundToInt(btn_pos[i].z)].transform.childCount == 0)
                {
                    buttons[i] = Instantiate(button_prefab, btn_pos[i], Quaternion.identity);
                    buttons[i].transform.parent = tiles[Mathf.RoundToInt(btn_pos[i].x), Mathf.RoundToInt(btn_pos[i].z)].transform;
                    btn_placed = true;
                }
            }
        }

        set_up = true;
    }
}