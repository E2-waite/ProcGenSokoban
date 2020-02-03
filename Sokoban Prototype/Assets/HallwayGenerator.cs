using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public class HallwayGenerator : MonoBehaviour
{
    public GameObject wall_prefab, floor_prefab;
    Templates templates;
    Vector2 start_point = new Vector2(0, 0);
    Vector2 start_dir = new Vector2(0, 0);
    public int num_segments = 1;
    List<GameObject> obj_grids = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        templates = GetComponent<Templates>();
        int[,] template = new int [5,5];
        StartCoroutine(LoadTemplate(template, Random.Range(0, 4), num_segments));
    }

    IEnumerator LoadTemplate(int[,] template, int temp_num, int steps_left)
    {
        Vector2 enterance_pos = new Vector2(0,0);
        Vector2 exit_pos = new Vector2(0, 0);
        template = templates.GetHall(temp_num);
        GameObject[,] grid = new GameObject[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (template[x, y] == (int)TILE.floor)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3((float)x, 0, (float)y), Quaternion.identity);
                }
                if (template[x, y] == (int)TILE.wall)
                {
                    grid[x, y] = Instantiate(wall_prefab, new Vector3((float)x, 1, (float)y), Quaternion.identity);
                }
                if (template[x, y] == (int)TILE.enterance)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3((float)x, 0, (float)y), Quaternion.identity);
                    enterance_pos = new Vector2((int)grid[x, y].transform.position.x, (int)grid[x, y].transform.position.z);
                }
                if (template[x, y] == (int)TILE.exit)
                {
                    grid[x, y] = Instantiate(floor_prefab, new Vector3((float)x, 0, (float)y), Quaternion.identity);
                    exit_pos = new Vector2((int)grid[x, y].transform.position.x, (int)grid[x, y].transform.position.z);
                }
            }
            yield return null;
        }
        steps_left--;
        if (steps_left > 0) StartCoroutine(LoadTemplate(template, Random.Range(0, 5), steps_left));
        else
        {
            Debug.Log("ENTERANCE POS: " + enterance_pos.x.ToString() + ":" + enterance_pos.y.ToString());
            Debug.Log("EXIT POS: " + exit_pos.x.ToString() + ":" + exit_pos.y.ToString());
        }
    }
}
