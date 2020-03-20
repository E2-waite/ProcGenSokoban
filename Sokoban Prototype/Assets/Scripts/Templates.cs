using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using enums;
public partial class Templates : MonoBehaviour
{
    public List<Template> templates = new List<Template>()
    {
        new Template // 0
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 1
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,1,1,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 2
        {
            template = new int[5,5]
            {
                {0,0,0,1,1},
                {0,2,2,1,1},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 3
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 4
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,2,1,1,0},
                {0,2,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 5
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,1,0},
                {1,1,1,1,0},
                {0,1,1,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 6
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,1,1,0},
                {1,1,1,1,0},
                {0,2,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 7
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,1,0},
                {1,1,1,1,0},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 8
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {1,1,1,1,1},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 9
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {0,2,1,1,1},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 10
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {1,1,1,1,1},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 11
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,1},
                {0,1,2,1,1},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 12
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,2,2,2,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 13
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,2,1,1,0},
                {1,1,1,1,0},
                {1,1,0,0,0}
            }
        },
        new Template // 14
        {
            template = new int[5,5]
            {
                {0,1,0,1,0},
                {0,1,1,1,0},
                {0,2,1,2,0},
                {0,1,1,1,0},
                {0,1,0,1,0}
            }
        },
        new Template // 15
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,2,2,2,0},
                {0,1,1,1,0},
                {0,1,1,1,0}
            }
        },
        new Template // 16
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {1,1,2,1,1},
                {0,1,1,1,0},
                {0,1,1,0,0}
            }
        },
    };



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

    static int[,] room_template_8 = new int[,]
    {
        
    };
    static int[,] room_template_9 = new int[,]
    {
        
    };
    static int[,] room_template_10 = new int[,]
    {
        
    };
    static int[,] room_template_11 = new int[,]
    {
        
    };
    static int[,] room_template_12 = new int[,]
    {
        
    };
    static int[,] room_template_13 = new int[,]
    {
        
    };
    static int[,] room_template_14 = new int[,]
    {
        
    };
    static int[,] room_template_15 = new int[,]
    {
        
    };
    static int[,] room_template_16 = new int[,]
    {
        
    };
    static int[,] room_template_17 = new int[,]
    {
        
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
