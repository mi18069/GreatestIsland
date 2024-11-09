using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Map
{
    public int width { get; private set; }
    public int height { get; private set; }
    public int maxHeightValue { get; private set; } = 1000;

    // Structure for cells
    private Cell[,] cellMap { get; set; }
    private IslandManager islandManager { get; set; }
    public void CreateMapFromMatrix(int[,] matrix)
    {
        height = matrix.GetLength(0);
        width = matrix.GetLength(1);

        cellMap = new Cell[height, width];

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                var cell = new Cell(x, y, matrix[x, y]);
                AddCellIntoMap(cell);
            }
        }

        GenerateIslands();
    }

    public Island GetCellIsland(Cell cell)
    {
        return islandManager.GetIslandById(cell.islandID);
    }

    public IEnumerable<Island> GetAllIslands()
    {
        return islandManager.GetAllIslands();
    }

    public void AddCellIntoMap(Cell cell)
    {
        cellMap[cell.position.x, cell.position.y] = cell;
    }

    public bool CheckIsland(Island island)
    {
        return islandManager.CheckIfIslandHaveGreatestAverageHeight(island);
    }

    public Cell GetCell(int x, int y)
    {
        if (IsValidCell(x, y))
            return cellMap[x, y];
        else
            return Cell.InvalidCell;
    }

    // Returns true if x and y coordinates are within map range
    private bool IsValidCell(int x, int y)
    {
        return (x >= 0 && x < height) && (y >= 0 && y < width);
    }

    // Find different unconnected Islands
    private void GenerateIslands()
    {
        islandManager = new IslandManager(this);
        islandManager.GenerateIslands();
    }    

}
