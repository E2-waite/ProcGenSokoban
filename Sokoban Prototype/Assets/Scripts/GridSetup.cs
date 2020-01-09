using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GridSetup : MonoBehaviour
{
    public int size_x = 4, size_y = 4;
    public int num_steps = 2;
    public int num_boxes = 2;
    private int num_tiles = 0;
    private int num_walls = 0;
    private bool set_up = false;
    public GameObject tile_prefab;
    public GameObject player_prefab;
    public GameObject box_prefab;
    public GameObject button_prefab;
    public GameObject wall_prefab;
    private GameObject[,] tiles;
    private GameObject player;
    private List<GameObject> boxes = new List<GameObject>();
    private List<GameObject> buttons = new List<GameObject>();
    private GameObject[] walls;
    private Vector3 pl_pos;
    private List<Vector3> box_pos = new List<Vector3>();
    private List<Vector3> btn_pos = new List<Vector3>();
    private Vector3[] wall_pos;
    public bool game_won = false;
    void Start()
    {
        num_tiles = size_x * size_y;
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

        StartCoroutine(RefreshRoutine());
    }

    void Update()
    {
        if (set_up)
        {
            if (CheckWin())
            {
                game_won = true;
            }
            else
            {
                game_won = false;
            }
        }

        if (game_won)
        {
            game_won = false;
            RefreshGame();
        }
    }

    bool CheckWin()
    {
        // Checks if all buttons have boxes on them
        for (int i = 0; i < boxes.Count; i++)
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
        if (set_up)
        {
            // Resets player and box to their original positions
            game_won = false;
            set_up = false;
            player.transform.position = pl_pos;
            player.transform.parent = tiles[Mathf.RoundToInt(pl_pos.x), Mathf.RoundToInt(pl_pos.z)].transform;
            PlayerMovement player_scr = player.GetComponent<PlayerMovement>();
            player_scr.x_pos = Mathf.RoundToInt(pl_pos.x);
            player_scr.y_pos = Mathf.RoundToInt(pl_pos.z);

            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].transform.parent = tiles[Mathf.RoundToInt(box_pos[i].x), Mathf.RoundToInt(box_pos[i].z)].transform;
                boxes[i].transform.position = box_pos[i];
                BoxMovement box_scr = boxes[i].GetComponent<BoxMovement>();
                box_scr.x_pos = Mathf.RoundToInt(box_pos[i].x);
                box_scr.y_pos = Mathf.RoundToInt(box_pos[i].z);
            }
            set_up = true;
        }
    }

    public void RefreshGame()
    {
        if (set_up)
        {
            StartCoroutine(RefreshRoutine());
        }
    }

    IEnumerator RefreshRoutine()
    {
        // Resets game with new number of boxes and positions
        game_won = false;
        set_up = false;
        if (player != null)
        {
            Destroy(player);
        }
        if (num_walls > 0)
        {
            for (int i = 0; i < num_walls; i++)
            {
                Destroy(walls[i]);
            }
        }
        if (boxes.Count > 0)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                Destroy(boxes[i]);

            }
            box_pos.Clear();
            boxes.Clear();
        }
        if (buttons.Count > 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }
            btn_pos.Clear();
            buttons.Clear();
        }

        num_walls = Random.Range(num_tiles / 10, num_tiles / 5);
        walls = new GameObject[num_walls];
        wall_pos = new Vector3[num_walls];
        for (int i = 0; i < num_walls; i++)
        {
            bool wall_placed = false;
            while (!wall_placed)
            {
                wall_pos[i] = new Vector3(Random.Range(0, size_x), 0.6f, Random.Range(0, size_y));
                if (tiles[Mathf.RoundToInt(wall_pos[i].x), Mathf.RoundToInt(wall_pos[i].x)].transform.childCount == 0)
                {
                    walls[i] = Instantiate(wall_prefab, wall_pos[i], Quaternion.identity);
                    walls[i].transform.parent = tiles[Mathf.RoundToInt(wall_pos[i].x), Mathf.RoundToInt(wall_pos[i].z)].transform;
                    wall_placed = true;
                }
                yield return null;
            }
        }

        for (int i = 0; i < 64; i++)
        {
            SetupBoxes();
        }

        if (boxes.Count >= num_boxes)
        {
            // Setup player in a random position
            bool player_placed = false;
            while (!player_placed)
            {
                pl_pos = new Vector3(Random.Range(0, size_x), 0.5f, Random.Range(0, size_y));
                if (tiles[Mathf.RoundToInt(pl_pos.x), Mathf.RoundToInt(pl_pos.x)].transform.childCount == 0)
                {
                    player = Instantiate(player_prefab, pl_pos, Quaternion.identity);
                    player.transform.parent = tiles[Mathf.RoundToInt(pl_pos.x), Mathf.RoundToInt(pl_pos.z)].transform;
                    PlayerMovement player_scr = player.GetComponent<PlayerMovement>();
                    player_scr.x_pos = Mathf.RoundToInt(pl_pos.x);
                    player_scr.y_pos = Mathf.RoundToInt(pl_pos.z);
                    player_placed = true;
                }
                yield return null;
            }
            set_up = true;
        }
        else
        {
            StartCoroutine(RefreshRoutine());
        }
    }

    bool SetupBoxes()
    {
        int x_pos, y_pos, x_start, y_start;
        bool can_place = false;
        x_pos = Random.Range(0, size_x);
        y_pos = Random.Range(0, size_y);
        x_start = x_pos;
        y_start = y_pos;
        if (tiles[x_pos, y_pos].transform.childCount == 0)
        {
            for (int j = 0; j < num_steps; j++)
            {
                DIRECTION dir = CheckTwo(x_pos, y_pos);

                if (dir == DIRECTION.up)
                {
                    y_pos--;
                }
                else if (dir == DIRECTION.right)
                {
                    x_pos++;
                }
                else if (dir == DIRECTION.down)
                {
                    y_pos++;
                }
                else if (dir == DIRECTION.left)
                {
                    x_pos--;
                }
                else if (dir == DIRECTION.none)
                {
                    can_place = false;
                    break;
                }
                can_place = true;
            }
            if (can_place && x_pos < size_x && x_pos > 0 && y_pos < size_y && y_pos > 0 &&
                x_pos != x_start && y_pos != y_start)
            {
                buttons.Add(Instantiate(button_prefab, new Vector3(x_start, 0.6f, y_start), Quaternion.identity));
                boxes.Add(Instantiate(box_prefab, new Vector3(x_pos, 0.6f, y_pos), Quaternion.identity));
                btn_pos.Add(new Vector3(x_start, 0.6f, y_start));
                box_pos.Add(new Vector3(x_pos, 0.6f, y_pos));
                buttons[buttons.Count - 1].transform.parent = tiles[x_start, y_start].transform;
                boxes[boxes.Count - 1].transform.parent = tiles[x_pos, y_pos].transform;
                BoxMovement box_scr = boxes[boxes.Count - 1].GetComponent<BoxMovement>();
                box_scr.x_pos = x_pos;
                box_scr.y_pos = y_pos;
                return true;
            }
        }
        
        return false;
    }

    DIRECTION CheckTwo(int x_pos, int y_pos)
    {
        DIRECTION dir = (DIRECTION)Random.Range(0, 3);
        int x_dir = 0, y_dir = 0;

        if (dir == DIRECTION.up)
        {
            x_dir = 0;
            y_dir = -1;
        }
        if (dir == DIRECTION.right)
        {
            x_dir = 1;
            y_dir = 0;
        }
        if (dir == DIRECTION.down)
        {
            x_dir = 0;
            y_dir = 1;
        }
        if (dir == DIRECTION.left)
        {
            x_dir = -1;
            y_dir = 0;
        }

        if (CheckEdge(x_pos + x_dir, y_pos + y_dir) && CheckEdge(x_pos + (x_dir * 2), y_pos + (y_dir * 2)))
        {
            if (tiles[x_pos + x_dir, y_pos + y_dir].transform.childCount == 0 &&
                tiles[x_pos + (x_dir * 2), y_pos + (y_dir * 2)].transform.childCount == 0)
            {
                return dir;
            }
        }

        return DIRECTION.none;
    }
}
