using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Island
{
    public enum State
    {
        Invalid,
        Default,
        Selected,
        Missed,
        Found
    }

    public static readonly Island InvalidIsland = new Island(State.Invalid, -1);

    public List<Cell> cells;
    public int id { get; private set; }
    public float averageHeight { get { return cells.Sum(c => c.height) / (float)cells.Count(); } }
    public State state;

    // For representing average height at the end of level
    public Vector3Int MedianCellPosition 
    => new Vector3Int(cells.Sum(c => c.position.x), cells.Sum(c => c.position.y), 0) / cells.Count();

    public Island(int id)
    {
        cells = new List<Cell>();
        this.id = id;
        this.state = State.Default;
    }
    // Private constructor for the invalid island
    private Island(State state, int id)
    {
        this.state = state;
        this.id = id;
    }

    public void AddCell(Cell cell)
    {
        cells.Add(cell);
    }

    public Vector3Int[] GetCellPositions()
    {
        return cells.Select(c => c.position).ToArray();
    }
}
