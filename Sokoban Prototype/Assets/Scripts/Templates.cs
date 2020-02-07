using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Templates : MonoBehaviour
{
    List<int[,]> room_templates = new List<int[,]>()
    {
        room_template_1,
        room_template_2,
        room_template_3,
        room_template_4,
        room_template_5,
        room_template_6,
        room_template_7,
        room_template_8,
        room_template_9,
        room_template_10,
        room_template_11,
        room_template_12,
        room_template_13,
        room_template_14,
        room_template_15,
        room_template_16,
        room_template_17
    };
    public int[,] GetRoomTemplate(int temp_num)
    {
        return room_templates[temp_num];
    }

    List<int[,]> hall_templates = new List<int[,]>()
    {
        hall_template_1,
        hall_template_2,
        hall_template_3,
        hall_template_4
    };
    public int[,] GetHallTemplate(int temp_num)
    {
        return hall_templates[temp_num];
    }


    static int[,] room_template_1 = new int[,]
    {
        {0,0,0,0,0},
        {0,1,1,1,0},
        {0,1,1,1,0},
        {0,1,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_2 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,1,1,0},
        {0,1,1,1,0},
        {0,1,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_3 = new int[,]
    {
        {0,0,0,1,1},
        {0,2,2,1,1},
        {0,1,1,1,0},
        {0,1,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_4 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {0,1,1,1,0},
        {0,1,1,1,0},
        {0,0,0,0,0}
    };

    static int[,] room_template_5 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {0,2,1,1,0},
        {0,2,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_6 = new int[,]
    {
        {0,0,1,0,0},
        {0,2,1,1,0},
        {1,1,1,1,0},
        {0,1,1,2,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_7 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,1,1,0},
        {1,1,1,1,0},
        {0,2,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_8 = new int[,]
    {
        {0,0,1,0,0},
        {0,2,1,1,0},
        {1,1,1,1,0},
        {0,2,1,2,0},
        {0,0,1,0,0}
    };
    static int[,] room_template_9 = new int[,]
    {
        {0,0,1,0,0},
        {0,2,1,2,0},
        {1,1,1,1,1},
        {0,2,1,2,0},
        {0,0,1,0,0}
    };
    static int[,] room_template_10 = new int[,]
    {
        {0,0,1,0,0},
        {0,2,1,2,0},
        {0,2,1,1,1},
        {0,2,2,2,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_11 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {1,1,1,1,1},
        {0,2,2,2,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_12 = new int[,]
    {
        {0,0,0,0,0},
        {0,1,1,1,1},
        {0,1,2,1,1},
        {0,1,1,1,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_13 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {0,2,2,2,0},
        {0,2,2,2,0},
        {0,0,0,0,0}
    };
    static int[,] room_template_14 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {0,2,1,1,0},
        {1,1,1,1,0},
        {1,1,0,0,0}
    };
    static int[,] room_template_15 = new int[,]
    {
        {0,1,0,1,0},
        {0,1,1,1,0},
        {0,2,1,2,0},
        {0,1,1,1,0},
        {0,1,0,1,0}
    };
    static int[,] room_template_16 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {0,2,2,2,0},
        {0,1,1,1,0},
        {0,1,1,1,0}
    };
    static int[,] room_template_17 = new int[,]
    {
        {0,0,0,0,0},
        {0,2,2,2,0},
        {1,1,2,1,1},
        {0,1,1,1,0},
        {0,1,1,0,0}
    };

    //Hallway Templates:
    static int[,] hall_template_1 = new int[,]
    {
        {0,0,0,0,0},
        {2,2,2,2,2},
        {3,1,1,1,4},
        {2,2,2,2,2},
        {0,0,0,0,0}
    };
    static int[,] hall_template_2 = new int[,]
    {
        {2,2,4,2,2},
        {2,1,1,1,2},
        {3,1,1,1,2},
        {2,1,1,1,2},
        {2,2,2,2,2}
    };
    static int[,] hall_template_3 = new int[,]
    {
        {0,2,4,2,0},
        {2,2,1,2,0},
        {3,1,1,2,0},
        {2,2,1,2,0},
        {0,2,4,2,0}
    };
    static int[,] hall_template_4 = new int[,]
    {
        {2,2,4,2,2},
        {2,1,1,1,2},
        {3,1,1,1,2},
        {2,1,1,1,2},
        {2,2,4,2,2}
    };
}
