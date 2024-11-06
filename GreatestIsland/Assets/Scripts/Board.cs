using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public HeightColorGradient heightColorGradient { get; private set; }
    public Tile tile;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        heightColorGradient = GetComponent<HeightColorGradient>();
    }

    public void Draw(Map map)
    {
        int width = map.width;
        int height = map.height;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Cell cell = map.GetCell(x, y);
                tilemap.SetTile(cell.position, GetTile(cell));
                tilemap.RefreshTile(cell.position);
            }
        }
    }

    public void RedrawCell(Map map, Cell cell)
    {
        map.AddCellIntoMap(cell);
        tilemap.SetTile(cell.position, GetTile(cell));
        tilemap.RefreshTile(cell.position);
    }

    private Tile GetTile(Cell cell)
    {
        Color cellColor = heightColorGradient.GetTileColor(cell);
        tile.color = cellColor;
        
        return tile;
    }

}
