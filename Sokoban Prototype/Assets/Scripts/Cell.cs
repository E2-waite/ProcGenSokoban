using enums;
public class Cell
{
    readonly bool[] walls = new bool[4] { true, true, true, true };
    readonly  int x_pos, y_pos;
    public Cell(int x, int y, Direction dir)
    {
        x_pos = x;
        y_pos = y;
        if (dir == Direction.N) walls[(int)Direction.S] = false;
        if (dir == Direction.E) walls[(int)Direction.W] = false;
        if (dir == Direction.S) walls[(int)Direction.N] = false;
        if (dir == Direction.W) walls[(int)Direction.E] = false;
    }
    public int GetX() { return x_pos; }
    public int GetY() { return y_pos; }
    public void ClearWall(Direction dir) => walls[(int)dir] = false;
    public bool[]GetWalls() { return walls; }
}
