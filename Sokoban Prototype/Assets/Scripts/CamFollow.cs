using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    GameObject player = null;
    public float height = 10;
    GameControl game;
    private void Start()
    {
        game = GameObject.FindGameObjectWithTag("Grid").GetComponent<GameControl>();
    }

    void Update()
    {
        if (game.game_started && player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            transform.position = new Vector3(5, height * 5, 5);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, height, player.transform.position.z);
        }
    }
}
