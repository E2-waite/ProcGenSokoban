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
        new Template // 17
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 18
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,1,2,0},
                {0,1,1,2,1},
                {0,0,0,1,1}
            }
        },
        new Template // 19
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {0,1,1,2,0},
                {0,1,1,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 20
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,1,1,2,0},
                {0,1,1,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 21
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,1,1,2,0},
                {0,1,1,1,1},
                {0,2,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 22
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 23
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {1,1,1,1,1},
                {0,2,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 24
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {0,2,1,1,1},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 25
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {0,2,1,2,0},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 26
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,2,1,0},
                {0,1,1,1,0},
                {0,0,1,1,0}
            }
        },
        new Template // 27
        {
            template = new int[5,5]
            {
                {1,1,0,0,0},
                {1,1,2,2,0},
                {0,1,1,2,0},
                {0,1,1,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 28
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,2,1,2,0},
                {1,1,1,1,1},
                {0,0,0,0,0}
            }
        },
        new Template // 29
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {1,1,2,2,0},
                {1,1,2,2,0},
                {1,1,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 30
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {1,1,1,2,0},
                {1,1,2,2,0},
                {0,1,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 31
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {1,2,2,1,0},
                {1,1,0,0,0}
            }
        },
        new Template // 32
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 33
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {0,1,1,2,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 34
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,1,1,0},
                {0,1,1,1,1},
                {0,1,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 35
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {0,1,1,1,1},
                {0,1,1,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 36
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {0,1,1,1,1},
                {0,1,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 37
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,2,0},
                {1,1,1,2,0},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 38
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {1,1,2,1,0},
                {1,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 39
        {
            template = new int[5,5]
            {
                {0,0,0,1,1},
                {0,1,1,1,1},
                {0,1,1,2,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 40
        {
            template = new int[5,5]
            {
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,2,2,2,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 41
        {
            template = new int[5,5]
            {
                {0,0,1,1,0},
                {0,1,1,1,0},
                {1,1,2,1,1},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 42
        {
            template = new int[5,5]
            {
                {1,1,0,0,0},
                {1,2,1,1,0},
                {0,2,1,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 43
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,1,1,0},
                {0,2,1,1,0},
                {0,2,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 44
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,1,1,0},
                {0,2,1,1,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 45
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {1,1,1,1,0},
                {0,2,1,1,0},
                {0,0,1,0,0}
            }
        },
        new Template // 46
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,1,0},
                {0,1,1,1,0},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 47
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,1,1,2,0},
                {1,1,1,1,1},
                {0,2,1,2,0},
                {0,0,1,0,0}
            }
        },
        new Template // 48
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,2,0},
                {1,1,1,2,0},
                {0,2,2,2,0},
                {0,0,0,0,0}
            }
        },
        new Template // 49
        {
            template = new int[5,5]
            {
                {0,1,1,0,0},
                {0,1,1,1,0},
                {0,1,2,1,0},
                {0,1,1,1,0},
                {0,0,0,0,0}
            }
        },
        new Template // 50
        {
            template = new int[5,5]
            {
                {0,0,0,1,1},
                {0,2,1,1,1},
                {0,2,1,1,0},
                {0,2,2,1,1},
                {0,0,0,1,1}
            }
        },
        new Template // 51
        {
            template = new int[5,5]
            {
                {0,0,0,0,0},
                {0,2,2,1,1},
                {0,2,2,1,1},
                {0,2,2,1,1},
                {0,0,0,0,0}
            }
        },
        new Template // 52
        {
            template = new int[5,5]
            {
                {0,0,1,0,0},
                {0,2,1,1,0},
                {0,2,2,1,1},
                {0,2,1,1,1},
                {0,0,1,0,0}
            }
        },
    };

    public Template GetTemplate (Direction dir)
    {
        // Gets a random template that's compatible with the desired direction of doorway
        if (dir == Direction.N)
        {
            List<int> compatible = new List<int>()
            {
                0,1,5,6,7,8,9,11,14,17,18,19,21,22,23,25,26,28,30,31,32,33,34,35,36,38,39,40,41,42,43,44,45,46,47,48,49,50,52
            };
            return templates[compatible[Random.Range(0, compatible.Count - 1)]];
        }
        if (dir == Direction.E)
        {
            List<int> compatible = new List<int>()
            {
                0,1,2,3,4,5,6,7,8,9,10,11,13,16,17,21,22,23,24,26,31,32,34,35,36,38,41,42,43,44,45,46,47,49,50,51,52
            };
            return templates[compatible[Random.Range(0, compatible.Count - 1)]];
        }
        if (dir == Direction.S)
        {
            List<int> compatible = new List<int>()
            {
                0,1,2,3,4,5,6,7,8,11,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,30,34,35,36,37,38,42,43,45,46,47,49,52
            };
            return templates[compatible[Random.Range(0, compatible.Count - 1)]];
        }
        if (dir == Direction.W)
        {
            List<int> compatible = new List<int>()
            {
                0,1,2,3,5,6,7,8,10,11,16,17,18,19,20,21,22,23,26,27,29,30,31,32,33,34,35,36,37,38,39,41,45,46,47,48,49
            };
            return templates[compatible[Random.Range(0, compatible.Count - 1)]];
        }
        return templates[Random.Range(0,templates.Count - 1)];
    }
}
