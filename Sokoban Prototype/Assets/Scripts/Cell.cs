using System.Collections;
using System.Collections.Generic;
using enums;
public class Cell
{
    readonly  Pos pos;
    public Direction entrance;
    public List<Direction> exits = new List<Direction>();
    public Cell(int x, int y, Direction dir)
    {
        pos = new Pos { x = x, y = y };
        if (dir == Direction.N) entrance = Direction.S;
        if (dir == Direction.E) entrance = Direction.W;
        if (dir == Direction.S) entrance = Direction.N;
        if (dir == Direction.W) entrance = Direction.E;
    }
    public Pos GetPos() { return pos; }
}
