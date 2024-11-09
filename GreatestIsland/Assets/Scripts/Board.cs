using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap cellTilemap { get; private set; }
    public Tilemap islandTilemap { get; private set; }
    public HeightColorGradient heightColorGradient { get; private set; }
    public IslandColorConverter islandColorConverter { get; private set; }
    public Tile tile;
    public GameObject borderPrefab;
    public GameObject textPrefab;
    public GameObject canvasForTextPrefab;

    private void Awake()
    {
        cellTilemap = transform.Find("CellTilemap").GetComponent<Tilemap>();
        islandTilemap = transform.Find("IslandTilemap").GetComponent<Tilemap>();
        heightColorGradient = GetComponentInChildren<HeightColorGradient>();
        islandColorConverter = GetComponentInChildren<IslandColorConverter>();

    }

    public void Draw(Map map)
    {

        cellTilemap.ClearAllTiles();
        islandTilemap.ClearAllTiles();
        HideIslandsAverageHeight();

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
        TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

        textComponent.transform.position = screenPosition + new Vector3(15, -15, 0);
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

        borderPrefab.transform.localScale = new Vector3(bounds.size.x + 0.5f, bounds.size.y + 0.5f, 1);
        borderPrefab.transform.position = borderPosition;
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
