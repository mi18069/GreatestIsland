using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap cellTilemap { get; private set; }
    public Tilemap islandTilemap { get; private set; }
    public HeightColorGradient heightColorGradient { get; private set; }
    public IslandColorConverter islandColorConverter { get; private set; }
    public Tile tile;


    private void Awake()
    {
        cellTilemap = transform.Find("CellTilemap").GetComponent<Tilemap>();
        islandTilemap = transform.Find("IslandTilemap").GetComponent<Tilemap>();
        heightColorGradient = GetComponentInChildren<HeightColorGradient>();
        islandColorConverter = GetComponentInChildren<IslandColorConverter>();

    }

    public void Draw(Map map)
    {

        DrawCells(map);
        DrawIslands(map);

    }

    private void DrawCells(Map map)
    {
        int width = map.width;
        int height = map.height;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Cell cell = map.GetCell(x, y);
                cellTilemap.SetTile(cell.position, GetCellTile(cell));
                cellTilemap.RefreshTile(cell.position);
            }
        }
    }

    private void DrawIslands(Map map)
    {
        var islands = map.GetAllIslands();
        foreach (var island in islands)
        {
            RedrawIsland(island);
        }
    }

    public void RedrawIsland(Island island)
    {
        var tile = GetIslandTile(island);
        foreach (var position in island.GetCellPositions())
        {
            islandTilemap.SetTile(position, tile);
            islandTilemap.RefreshTile(position);
        }
    }

    private Tile GetCellTile(Cell cell)
    {
        Color cellColor = heightColorGradient.GetTileColor(cell);
        tile.color = cellColor;
        
        return tile;
    }

    private Tile GetIslandTile(Island island)
    {
        Color cellColor = islandColorConverter.Convert(island);
        tile.color = cellColor;

        return tile;
    }

}
