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
}
