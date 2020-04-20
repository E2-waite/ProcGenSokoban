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
    public Cell(int x, int y, Direction dir, Cell parent_cell = null)
    {
        parent = parent_cell;
        pos = new Pos { x = x, y = y };
        if (dir == Direction.N) entrance = Direction.S;
        if (dir == Direction.E) entrance = Direction.W;
        if (dir == Direction.S) entrance = Direction.N;
        if (dir == Direction.W) entrance = Direction.E;
    }
    public void AddParentExit()
    {
        if (entrance == Direction.N) parent.exits.Add(Direction.S);
        if (entrance == Direction.E) parent.exits.Add(Direction.W);
        if (entrance == Direction.S) parent.exits.Add(Direction.N);
        if (entrance == Direction.W) parent.exits.Add(Direction.E);
    }
}
