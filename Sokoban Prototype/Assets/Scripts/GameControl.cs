using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using enums;
public class GameControl : MonoBehaviour
{
    public GameObject[,] object_grid;
    public bool game_started = false;
    public float seconds = 0;
    GenerateLevel level;
    bool time_written = false;
    float time = 0;
    public GameObject player;
    private void Start()
    {
        level = GetComponent<GenerateLevel>();
    }

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

    public void StartGame(GameObject[,] obj_grid)
    {
        game_started = true;
        object_grid = obj_grid;

        string row = null;

        for (int i = 0; i < object_grid.GetLength(0); i++)
        {
            row += object_grid[i, 4].name + " ";
        }
        Debug.Log(row);
    }

    public void UpdatePosition(Elements element, int[] old_pos, int[] new_pos)
    {
        level.grid[old_pos[0], old_pos[1]] -= (int)element;
        level.grid[new_pos[0], new_pos[1]] += (int)element;
    }
}
