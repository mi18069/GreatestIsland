public class Map
{
    public int width { get; private set; }
    public int height { get; private set; } 
    public int maxHeightValue { get; private set; }

    // Structure for cells
    private Cell[,] cellMap { get; set; }

    public Map(int height, int width, int maxHeightValue)
    {
        this.height = height;
        this.width = width;
        cellMap = new Cell[height, width];
        this.maxHeightValue = maxHeightValue;
    }

    public void AddCellIntoMap(Cell cell)
    {
        cellMap[cell.position.x, cell.position.y] = cell;
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

}
