using Unity.VisualScripting;
using UnityEngine;

public class Cell
{
    public enum Type
    {
        Invalid,
        Water,
        Land
    }

    public Type type;
    public Vector3Int position;
    public int height;
    // Island manager is the one that changes and uses this variable
    // Consider other approaches to fix the problem when we need to retrieve the island from selected cell
    public int islandID = -1;

    // Singleton instance of an invalid cell, used for retrieving invalid cell instead of null
    public static readonly Cell InvalidCell = new Cell(Type.Invalid, new Vector3Int(0, 0, 0), 0);

    public Cell(int x, int y, int height)
    {
        this.height = height;
        this.position = new Vector3Int(x, y, 0);
        switch (height)
        {
            case 0:
                this.type = Type.Water; break;
            default:
                this.type = Type.Land; break;
        }
    }

    // Private constructor for the invalid cell
    private Cell(Type type, Vector3Int position, int height)
    {
        this.type = type;
        this.position = position;
        this.height = height;
    }
}
