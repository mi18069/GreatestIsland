using UnityEngine;

public class Cell
{
    public enum Type
    {
        Water,
        Land
    }

    public Type type;
    public Vector3Int position;
    public int height;
    public bool selected = false;

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
}
