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
    bool lined_up, flipped;
    void Start()
    {
        templates = GetComponent<Templates>();
        template = new int [5,5];
        StartCoroutine(LoadTemplate(Random.Range(0, 4), num_segments, start_dir, start_pos));
    }

    IEnumerator LoadTemplate(int temp_num, int steps_left, Vector2 dir, Vector2 pos)
    {
        float x_offset = 0, y_offset = 0;
        template = templates.GetHall(temp_num);
        lined_up = false;
        flipped = false;

        // Flip template 50% of the time
        bool flip_template = (Random.value > 0.5f);
        if (flip_template) FlipTemplate();
        else flipped = true;
        while (!flipped) yield return null;

        //Wait until section is lined up
        StartCoroutine(RotateTemplate(dir));
        while (!lined_up) yield return null;

        // Calculate section offset
        if (dir == Vector2.right)
        {
            x_offset = (int)pos.x + 5;
            y_offset = (int)pos.y;
        }
        if (dir == Vector2.left)
        {
            x_offset = (int)pos.x - 5;
            y_offset = (int)pos.y;
        }
        if (dir == Vector2.up)
        {
            x_offset = (int)pos.x;
            y_offset = (int)pos.y + 5;
        }
        if (dir == Vector2.down)
        {
            x_offset = (int)pos.x;
            y_offset = (int)pos.y - 5;
        }

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
                    if (y == 0) dir = Vector2.down;
                    if (y == 4) dir = Vector2.up; 
                    grid[x, y].name = "EXIT";
                }
            }
            yield return null;
        }
        obj_grids.Add(grid);

        pos = new Vector2(x_offset, y_offset);
        steps_left--;
        if (steps_left > 0) StartCoroutine(LoadTemplate(Random.Range(0, 4), steps_left, dir, pos));
        else Debug.Log("HALLWAY FINISHED");
    }

    private void FlipTemplate()
    {
        // Flip along x axis
        int[,] flipped_template = new int[5, 5];
        for (int y = 0; y < 5; y++)
        {
            int new_x = 4;
            for (int x = 0; x < 5; x++)
            {
                flipped_template[x, y] = template[new_x, y];
                new_x--;
            }
        }
        template = flipped_template;
        flipped = true;
    }

    IEnumerator RotateTemplate(Vector2 dir)
    {
        int[,] temp_template = template;
        Vector2 desired_start = new Vector2(0,0);
        if (dir == Vector2.right) desired_start = new Vector2(0, 2);
        if (dir == Vector2.left) desired_start = new Vector2(4, 2);
        if (dir == Vector2.up) desired_start = new Vector2(2, 0);
        if (dir == Vector2.down) desired_start = new Vector2(2, 4);

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
    }

    static int[,] RotateTemplate(int[,] template)
    {
        // Rotate template 90 degrees clockwise
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
