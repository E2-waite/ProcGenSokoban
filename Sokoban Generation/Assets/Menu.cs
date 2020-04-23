using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    public GameObject controller;
    GameControl game_control;
    public Button gen_button;
    public InputField room_x, room_y, maze_x, maze_y;
    public int min = 1, max = 5;
    bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        game_control = controller.GetComponent<GameControl>();
    }

    public void ToggleMenu()
    {
        if (active)
        {
            active = false;
        }
        else
        {
            active = true;
        }
        gen_button.gameObject.SetActive(active);
        room_x.gameObject.SetActive(active);
        room_y.gameObject.SetActive(active);
        maze_x.gameObject.SetActive(active);
        maze_y.gameObject.SetActive(active);
    }


    private void Update()
    {
        int out_i;
        if (int.TryParse(room_x.text, out out_i))
        {
            game_control.size_x = Mathf.Clamp(int.Parse(room_x.text), min, max);
        }
        if (int.TryParse(room_y.text, out out_i))
        {
            game_control.size_y = Mathf.Clamp(int.Parse(room_y.text), min, max);
        }
        if (int.TryParse(maze_x.text, out out_i))
        {
            game_control.maze_x = Mathf.Clamp(int.Parse(maze_x.text), min, max);
        }
        if (int.TryParse(maze_y.text, out out_i))
        {
            game_control.maze_y = Mathf.Clamp(int.Parse(maze_y.text), min, max);
        }
    }
}
