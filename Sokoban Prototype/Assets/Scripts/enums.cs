namespace enums
{
    public enum DIRECTION
    {
        up,
        right,
        down, 
        left,
        none
    }

    public enum TILE : int 
    { 
        blank = 0,
        floor = 1,
        wall = 2,
        enterance = 3,
        exit = 4
    }

    public enum Direction : int
    {
        N = 0,
        E = 1,
        S = 2,
        W = 3
    }
}
