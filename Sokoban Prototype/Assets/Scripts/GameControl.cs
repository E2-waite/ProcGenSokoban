using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using enums;
public class GameControl : MonoBehaviour
{
    public bool game_won = false, game_started = false;
    public float seconds = 0;
    bool time_written = false;
    int[,] grid;
    public GameObject[,] object_grid;
    int num_boxes;
    float time = 0;
    private void Update()
    {
        if (!game_started)
        {
            time += Time.deltaTime;
            seconds = time % 60;
        }
        else
        {
            if (!time_written)
            {
                time_written = true;
                WriteTime();
            }
        }
    }

    void WriteTime()
    {
        Debug.Log("Writing time");
        StreamWriter writer = new StreamWriter("Assets/GenerationTime.txt", true);
        writer.WriteLine(seconds.ToString());
        writer.Close();
    }

    public void StartGame(Room room, int boxes)
    {
        Debug.Log("GAME STARTED, Num Boxes: " + boxes.ToString());
        game_won = false;
        game_started = true;
        grid = room.grid;
        object_grid = room.object_grid;
        num_boxes = boxes;
        StartCoroutine(CheckWin(room));
    }

    IEnumerator CheckWin(Room room)
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
            GetComponent<GenerateGrid>().Restart(room);

        }
        else
        {
            game_won = false;
            StartCoroutine(CheckWin(room));
        }
    }

    public void UpdatePosition(Elements element, int[] old_pos, int[] new_pos)
    {
        grid[old_pos[0], old_pos[1]] -= (int)element;
        grid[new_pos[0], new_pos[1]] += (int)element;
    }
}
