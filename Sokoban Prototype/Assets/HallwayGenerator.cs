using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class HallwayGenerator : MonoBehaviour
{
    public GameObject wall_prefab, floor_prefab;
    Templates templates;
    Vector2 start_pos = new Vector2(0, 0);
    Vector2 start_dir = new Vector2(0, 0);
    public int num_segments = 1;
    List<GameObject> obj_grids = new List<GameObject>();

    void Start()
    {
        templates = GetComponent<Templates>();
        int[,] template = new int [5,5];
        StartCoroutine(LoadTemplate(template, Random.Range(0, 4), num_segments, start_dir, start_pos));
    }

    IEnumerator LoadTemplate(int[,] template, int temp_num, int steps_left, Vector2 dir, Vector2 last_pos)
    {
        float x_offset = 0, y_offset = 0;
        Vector2 enterance_pos = new Vector2(0,0);
        Vector2 exit_pos = new Vector2(0, 0);
        template = templates.GetHall(temp_num);

        if (dir == Vector2.right)
        {
            x_offset = (int)last_pos.x + 5;
            y_offset = (int)last_pos.y;
        }
        if (dir == Vector2.left)
        {
            x_offset = (int)last_pos.x - 5;
            y_offset = (int)last_pos.y;
        }
        if (dir == Vector2.up)
        {
            x_offset = (int)last_pos.x;
            y_offset = (int)last_pos.y + 5;
        }
        if (dir == Vector2.down)
        {
            x_offset = (int)last_pos.x;
            y_offset = (int)last_pos.y - 5;
        }
        last_pos = new Vector2(last_pos.x + x_offset, last_pos.y + y_offset);

        GameObject[,] grid = new GameObject[5, 5];
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (template[x, y] == (int)TILE.floor)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3(x + x_offset, 0, y + y_offset), Quaternion.identity);
                }
                if (template[x, y] == (int)TILE.wall)
                {
                    grid[x, y] = Instantiate(wall_prefab, new Vector3(x + x_offset, 1, y + y_offset), Quaternion.identity);
                }
                if (template[x, y] == (int)TILE.enterance)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3(x + x_offset, 0, y + y_offset), Quaternion.identity);
                    enterance_pos = new Vector2((int)grid[x, y].transform.position.x, (int)grid[x, y].transform.position.z);
                    grid[x, y].name = "ENTRANCE";
                }
                if (template[x, y] == (int)TILE.exit)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3(x + x_offset, 0, y + y_offset), Quaternion.identity);
                    exit_pos = new Vector2((int)grid[x, y].transform.position.x, (int)grid[x, y].transform.position.z);
                    if (x == 0) dir = Vector2.left;
                    if (x == 4) dir = Vector2.right;
                    if (y == 0) dir = Vector2.up;
                    if (y == 4) dir = Vector2.down;
                    grid[x, y].name = "EXIT";
                }
            }
            yield return null;
        }
        Debug.Log("EXIT DIRECTION: " + dir.ToString());
        steps_left--;
        if (steps_left > 0) StartCoroutine(LoadTemplate(template, Random.Range(0, 4), steps_left, dir, last_pos));
        else
        {
            Debug.Log("ENTERANCE POS: " + enterance_pos.x.ToString() + ":" + enterance_pos.y.ToString());
            Debug.Log("EXIT POS: " + exit_pos.x.ToString() + ":" + exit_pos.y.ToString());
        }
    }
}
