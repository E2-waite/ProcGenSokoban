using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class GameControl : MonoBehaviour
{
    public bool game_won = false;
    int[,] grid;
    public GameObject[,] object_grid;
    int num_boxes;
    
    public void StartGame(int[,] i_grid, GameObject[,] o_grid, int boxes)
    {
        Debug.Log("GAME STARTED, Num Boxes: " + boxes.ToString());
        game_won = false;
        grid = i_grid;
        object_grid = o_grid;
        num_boxes = boxes;
        StartCoroutine(CheckWin());
    }

    IEnumerator CheckWin()
    {
        int buttons_pressed = 0;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == (int)Elements.floor + (int)Elements.button + (int)Elements.box)
                {
                    buttons_pressed++;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (buttons_pressed == num_boxes)
        {
            Debug.Log("GAME WON");
            game_won = true;
            GetComponent<GenerateGrid>().Restart();

        }
        else
        {
            game_won = false;
            StartCoroutine(CheckWin());
        }
    }

    public void UpdatePosition(Elements element, int[] old_pos, int[] new_pos)
    {
        grid[old_pos[0], old_pos[1]] -= (int)element;
        grid[new_pos[0], new_pos[1]] += (int)element;
    }
}
