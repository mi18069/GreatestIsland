public class Map
{
    public int width { get; private set; }
    public int height { get; private set; } 
    public int maxHeightValue { get; private set; }

    public Cell[,] cellMap { get; private set; }

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

    public Cell GetCellByCoordinates(int x, int y)
    {
        return cellMap[x, y];
    }

}
