using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    public GameObject controller;
    GameControl game_control;
    public List<Text> text_list = new List<Text>();
    public List<Image> backgrounds = new List<Image>();
    public Button gen_button, open_button, close_button;
    public Slider room_x, room_y, maze_x, maze_y, max_depth, steps_back, max_rooms;
    public Image closed_bg;
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
        foreach(Text text in text_list)
        {
            text.gameObject.SetActive(active);
        }
        foreach(Image background in backgrounds)
        {
            background.gameObject.SetActive(active);
        }
        gen_button.gameObject.SetActive(active);
        room_x.gameObject.SetActive(active);
        room_y.gameObject.SetActive(active);
        maze_x.gameObject.SetActive(active);
        maze_y.gameObject.SetActive(active);
        max_depth.gameObject.SetActive(active);
        steps_back.gameObject.SetActive(active);
        max_rooms.gameObject.SetActive(active);
        close_button.gameObject.SetActive(active);
        closed_bg.gameObject.SetActive(!active);
        open_button.gameObject.SetActive(!active);
    }


    private void Update()
    { 
        if (game_control.size_x != (int)room_x.value)
        {
            game_control.size_x = (int)room_x.value;
        }
        if (game_control.size_y != (int)room_y.value)
        {
            game_control.size_y = (int)room_y.value;
        }
        if (game_control.maze_x != (int)maze_x.value)
        {
            game_control.maze_x = (int)maze_x.value;
        }
        if (game_control.maze_y != (int)maze_y.value)
        {
            game_control.maze_y = (int)maze_y.value;
        }
        if (game_control.max_depth != (int)max_depth.value)
        {
            game_control.max_depth = (int)max_depth.value;
        }
        if (game_control.steps_back != (int)steps_back.value)
        {
            game_control.steps_back = (int)steps_back.value;
        }
        if (game_control.max_rooms != (int)max_rooms.value)
        {
            game_control.max_rooms = (int)max_rooms.value;
        }
    }
}
