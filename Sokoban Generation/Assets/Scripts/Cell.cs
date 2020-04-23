using System.Collections;
using System.Collections.Generic;
using enums;
public class Cell
{
    public Cell parent;
    public Pos pos;
    public Direction entrance = Direction.None;
    public List<Direction> exits = new List<Direction>();
    public bool first_room = false;
    public bool last_room = false;
    public int depth = 0;
    public bool stepped = false;
    public Cell(int x, int y, Direction dir, Cell parent_cell = null)
    {
        parent = parent_cell;
        pos = new Pos { x = x, y = y };
        if (dir == Direction.N) entrance = Direction.S;
        if (dir == Direction.E) entrance = Direction.W;
        if (dir == Direction.S) entrance = Direction.N;
        if (dir == Direction.W) entrance = Direction.E;
    }
    public void AddParentExit(Direction dir)
    {
        if (dir == Direction.N) exits.Add(Direction.S);
        if (dir == Direction.E) exits.Add(Direction.W);
        if (dir == Direction.S) exits.Add(Direction.N);
        if (dir == Direction.W) exits.Add(Direction.E);
    }
}
