public class Map
{
    public int width { get; private set; }
    public int height { get; private set; } 
    public int maxHeightValue { get; private set; }

    public Cell[,] cellMap { get; private set; }

    public Map(int width, int height, int maxHeightValue)
    {
        this.width = width;
        this.height = height;
        cellMap = new Cell[width, height];
        this.maxHeightValue = maxHeightValue;
    }

    public void AddCellIntoMap(Cell cell)
    {
        cellMap[cell.position.x, cell.position.y] = cell;
    }

    public Cell GetCellByCoordinates(int x, int y)
    {
        return cellMap[x, y];
    }

}
