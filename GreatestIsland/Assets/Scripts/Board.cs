using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap cellTilemap { get; private set; }
    public Tilemap islandTilemap { get; private set; }
    public Tilemap fogTilemap { get; private set; }
    public HeightColorGradient heightColorGradient { get; private set; }
    public IslandColorConverter islandColorConverter { get; private set; }
    public Tile tile;
    public GameObject tilemapBorder;
    public GameObject textPrefab;
    public GameObject canvasForTextPrefab;

    private Vector3Int[,] fogMatrix = new Vector3Int[15, 15];

    private void Awake()
    {
        cellTilemap = transform.Find("CellTilemap").GetComponent<Tilemap>();
        islandTilemap = transform.Find("IslandTilemap").GetComponent<Tilemap>();
        fogTilemap = transform.Find("FogTilemap").GetComponent<Tilemap>();
        heightColorGradient = GetComponentInChildren<HeightColorGradient>();
        islandColorConverter = GetComponentInChildren<IslandColorConverter>();

    }

    public void Draw(Map map)
    {

        cellTilemap.ClearAllTiles();
        islandTilemap.ClearAllTiles();
        HideIslandsAverageHeight();
        tilemapBorder.SetActive(false);
        DrawCells(map);
        DrawIslands(map);
        SetBorder();
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

    public void DrawFog(Map map)
    {
        fogTilemap.ClearAllTiles();
        int width = map.width;
        int height = map.height;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Cell cell = map.GetCell(x, y);
                fogTilemap.SetTile(cell.position, GetFogTile());
                fogTilemap.RefreshTile(cell.position);
            }
        }
    }

    public void HideFog()
    {
        fogTilemap.ClearAllTiles();
    }

    public void FogCenterCellChanged(Cell centerCell, Map map)
    {
        int height = fogMatrix.GetLength(0);
        int width = fogMatrix.GetLength(1);

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Cell cell = map.GetCell(fogMatrix[x, y].x, fogMatrix[x,y].y);
                if (cell.type == Cell.Type.Invalid)
                    continue;
                fogTilemap.SetTile(cell.position, GetFogTile());
                fogTilemap.RefreshTile(cell.position);
            }
        }

        if (centerCell.type == Cell.Type.Invalid)
            return;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int xPos = centerCell.position.x + x - height / 2;
                int yPos = centerCell.position.y + y - width / 2;

                Cell cell = map.GetCell(xPos, yPos);
                if (cell.type == Cell.Type.Invalid)
                    continue;

                fogTilemap.SetTile(cell.position, GetFogTransparentTile(Mathf.Abs(x - height/2) + Mathf.Abs(y - width/2)));
                fogMatrix[x, y] = cell.position;
                fogTilemap.RefreshTile(cell.position);

            }
        }

    }



    public void ShowIslandsAverageHeight(List<Island> islands)
    {
        foreach (var island in islands)
        {
            ShowIslandAverageHeight(island);
        }
    }

    private void ShowIslandAverageHeight(Island island)
    {
        var medianCellPosition = island.MedianCellPosition;
        Vector3 worldPosition = islandTilemap.CellToWorld(medianCellPosition);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        GameObject textInstance = Instantiate(textPrefab, screenPosition, Quaternion.identity);
        textInstance.transform.SetParent(canvasForTextPrefab.transform, false);
        textInstance.tag = "TextPrefab"; // Important because later it will be deleted by this attribute
        textInstance.transform.position = screenPosition;
        textInstance.SetActive(true);

        TextMeshProUGUI textComponent = textInstance.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = Mathf.RoundToInt(island.averageHeight).ToString();
    }

    private void HideIslandsAverageHeight()
    {
        GameObject[] textObjects = GameObject.FindGameObjectsWithTag("TextPrefab");

        foreach (GameObject text in textObjects)
        {
            Destroy(text);
        }
    }

    void SetBorder()
    {
        BoundsInt bounds = cellTilemap.cellBounds;
        Vector3 borderPosition = cellTilemap.GetComponent<Renderer>().bounds.center;

        tilemapBorder.SetActive(true);
        tilemapBorder.transform.localScale = new Vector3(bounds.size.x + 0.5f, bounds.size.y + 0.5f, 1);
        tilemapBorder.transform.position = borderPosition;
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

    private Tile GetFogTile()
    {

        Color cellColor = Color.gray;
        cellColor.a = 0.8f;
        tile.color = cellColor;

        return tile;
    }

    private Tile GetFogTransparentTile(int distance)
    {
        Color cellColor = Color.gray;
        float alpha;
        if (distance <= 7)
            alpha = 0;
        else
            alpha = (distance - 7) * 0.07f;

        cellColor.a = alpha;
        tile.color = cellColor;

        return tile;
    }

}
