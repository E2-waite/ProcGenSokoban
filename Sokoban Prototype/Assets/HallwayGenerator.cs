using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class HallwayGenerator : MonoBehaviour
{
    public GameObject wall_prefab, floor_prefab;
    Templates templates;
    int[,] template;
    Vector2 start_pos = new Vector2(0, 0);
    Vector2 start_dir = new Vector2(0, 1);
    public int num_segments = 1;
    List<GameObject[,]> obj_grids = new List<GameObject[,]>();
    bool lined_up;
    void Start()
    {
        templates = GetComponent<Templates>();
        template = new int [5,5];
        StartCoroutine(LoadTemplate(Random.Range(0, 4), num_segments, start_dir, start_pos));
    }

    IEnumerator LoadTemplate(int temp_num, int steps_left, Vector2 dir, Vector2 last_pos)
    {
        float x_offset = 0, y_offset = 0;
        Vector2 entrance_pos = new Vector2(0,0);
        Vector2 exit_pos = new Vector2(0, 0);
        template = templates.GetHall(temp_num);
        lined_up = false;
        // Find entrance and exit positions
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (template[x, y] == (int)TILE.enterance)
                {
                    entrance_pos = new Vector2(x, y);
                }
                if (template[x, y] == (int)TILE.exit)
                {
                    exit_pos = new Vector2(x, y);
                }
            }
        }

        StartCoroutine(RotateTemplate(dir));
        //Wait until section is lined up
        while (!lined_up) yield return null;

        // Calculate section offset
        if (dir == Vector2.right)
        {
            x_offset = (int)last_pos.x + 5;
            y_offset = 0;
        }
        if (dir == Vector2.left)
        {
            x_offset = (int)last_pos.x - 5;
            y_offset = 0;
        }
        if (dir == Vector2.up)
        {
            x_offset = 0;
            y_offset = (int)last_pos.y + 5;
        }
        if (dir == Vector2.down)
        {
            x_offset = 0;
            y_offset = (int)last_pos.y - 5;
        }
        last_pos = new Vector2(last_pos.x + x_offset, last_pos.y + y_offset);

        // Generate hallway section
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
                    grid[x, y].name = "ENTRANCE";
                }
                if (template[x, y] == (int)TILE.exit)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3(x + x_offset, 0, y + y_offset), Quaternion.identity);
                    if (x == 0) dir = Vector2.left;
                    if (x == 4) dir = Vector2.right;
                    if (y == 0) dir = Vector2.up;
                    if (y == 4) dir = Vector2.down;
                    grid[x, y].name = "EXIT";
                }
            }
            yield return null;
        }
        obj_grids.Add(grid);

        steps_left--;
        if (steps_left > 0) StartCoroutine(LoadTemplate(Random.Range(0, 4), steps_left, dir, last_pos));
        else
        {
            Debug.Log("FINISHED");
        }
    }

    IEnumerator RotateTemplate(Vector2 dir)
    {
        Debug.Log("STARTED ROTATION, DIR: " + dir.x.ToString() + dir.y.ToString());
        int[,] temp_template = template;
        Vector2 desired_start = new Vector2(0,0);
        if (dir == Vector2.right) desired_start = new Vector2(0, 2);
        if (dir == Vector2.left) desired_start = new Vector2(4, 2);
        if (dir == Vector2.up) desired_start = new Vector2(2, 0);
        if (dir == Vector2.down) desired_start = new Vector2(2, 4);
        Debug.Log("DESIRED START: " + desired_start.x.ToString() + desired_start.y.ToString());

        // Check if current orientation's entrace matches previous section's exit
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (temp_template[x, y] == (int)TILE.enterance && x == desired_start.x && y == desired_start.y)
                {
                    template = temp_template;
                    lined_up = true;
                    yield break;
                }
                yield return null;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            temp_template = RotateTemplate(temp_template);

            // Check if current orientation's entrace matches previous section's exit
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (temp_template[x, y] == (int)TILE.enterance)
                    {
                        Debug.Log("X: " + x.ToString() + " Y: " + y.ToString());
                        if (x == desired_start.x && y == desired_start.y)
                        {
                            template = temp_template;
                            lined_up = true;
                            yield break;
                        }
                    }
                    yield return null;
                }
            }
        }
        Debug.Log("FAILED ROTATION");
    }

    static int[,] RotateTemplate(int[,] template)
    {
        int[,] rot_template = new int[5, 5];

        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                rot_template[i, j] = template[5 - j - 1, i];
            }
        }

        return rot_template;
    }
}
