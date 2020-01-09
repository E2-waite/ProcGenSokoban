using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GridTemplate
{
    private int[,] template;
    private int grid_num;
    public GridTemplate(GameObject grid)
    {
        int grid_num = Random.Range(0, 17);
        template = new int[5, 5];
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                template[x, y] = grid.GetComponent<Templates>().Get(grid_num, x, y);
            }
        }
        RotateTemplate();
    }

    private void RotateTemplate()
    {
        int num_rotations = Random.Range(0, 4);
        int amount_rotated = 0;
        int[,] temp_template = new int[5, 5];
        temp_template = template;

        for (int i = 0; i < num_rotations; i++)
        {
            int[,] rot_template = new int[5, 5];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    rot_template[x, y] = temp_template[y, x];
                }
            }
            amount_rotated += 90;
            temp_template = rot_template;
        }

        template = temp_template;

        FlipTemplate();
    }

    private void FlipTemplate()
    {
        int flip_type = Random.Range(0, 3);
        int[,] flipped_template = new int[5, 5];

        if (flip_type == 0)
        {
            return;
        }
        else if (flip_type == 1)
        {
            for (int y = 0; y < 5; y++)
            {
                int new_x = 4;
                for (int x = 0; x < 5; x++)
                {
                    flipped_template[x, y] = template[new_x, y];
                    new_x--;
                }
            }
        }
        else if (flip_type == 2)
        {
            for (int y = 0; y < 5; y++)
            {
                int new_y = 4;
                for (int x = 0; x < 5; x++)
                {
                    flipped_template[x, y] = template[x, new_y];
                    new_y--;
                }
            }
        }

        template = flipped_template;
    }

    public int GetTemplate(int x, int y)
    {
        return template[x, y];
    }
}
