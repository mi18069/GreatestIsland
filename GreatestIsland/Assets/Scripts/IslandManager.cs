using System;
using System.Collections.Generic;
using System.Linq;

public class IslandManager
{
    private List<Island> islands;
    private Map map;

    // For the case where two or more Islands have the same max average height, we need List, not one instance
    private List<Island> greatestAverageHeightIsland;

    public IslandManager(Map map)
    {
        this.islands = new List<Island>();
        this.map = map;
        greatestAverageHeightIsland = new List<Island>();
    }

    // If no island with given id is found, return NullIsland instance
    public Island GetIslandById(int id)
    {
        if (id == -1)
            return Island.InvalidIsland;

        return islands.FirstOrDefault(i => i.id == id) ?? Island.InvalidIsland;
    }

    public List<Island> GetAllIslands()
    {
        return islands;
    }

    public bool CheckIfIslandHaveGreatestAverageHeight(Island island)
    {
        return greatestAverageHeightIsland.Contains(island);
    }

    private void FindGreatestIsland()
    {
        float maxAverageHeight = islands.Max(i => i.averageHeight);
        greatestAverageHeightIsland = islands.Where(i => Math.Abs(i.averageHeight - maxAverageHeight) < 0.001).ToList();
    }

    // Find group of separate islands 
    public void GenerateIslands()
    {
        int height = map.height;
        int width = map.width;  

        bool[,] visited = new bool[height, width];

        int islandId = 1;
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Cell cell = map.GetCell(x, y);
                if (cell.type == Cell.Type.Land && !visited[x, y])
                {
                    Island island = new Island(islandId++);
                    ExploreIsland(cell, island, visited);
                    islands.Add(island);
                }
            }
        }

        FindGreatestIsland();
    }

    // Use BFS search to find all connected land cells from starting cell and place them into one Island
    private void ExploreIsland(Cell startingCell, Island island, bool[,] visited)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(startingCell);

        while (queue.Count > 0)
        {
            Cell cell = queue.Dequeue();
            if (visited[cell.position.x, cell.position.y])
                continue;

            visited[cell.position.x, cell.position.y] = true;
            // Adding each cell an islandID for quick lookup 
            cell.islandID = island.id;
            island.AddCell(cell);

            var neighbourCells = GetNeighborCells(cell);
            foreach (var neighbourCell in neighbourCells)
            {
                if (neighbourCell.type == Cell.Type.Land)
                    queue.Enqueue(neighbourCell);
            }
        }
    }

    // Return only cells with type land from top, bottom, left and right dirrection
    private IEnumerable<Cell> GetNeighborCells(Cell cell)
    {
        List<Cell> neighbourCells = new List<Cell>();
        Cell topCell = map.GetCell(cell.position.x - 1, cell.position.y);
        Cell bottomCell = map.GetCell(cell.position.x + 1, cell.position.y);
        Cell leftCell = map.GetCell(cell.position.x, cell.position.y - 1);
        Cell rightCell = map.GetCell(cell.position.x, cell.position.y + 1);

        if (topCell.type == Cell.Type.Land)
            neighbourCells.Add(topCell);
        if (bottomCell.type == Cell.Type.Land)
            neighbourCells.Add(bottomCell);
        if (leftCell.type == Cell.Type.Land)
            neighbourCells.Add(leftCell);
        if (rightCell.type == Cell.Type.Land)
            neighbourCells.Add(rightCell);

        return neighbourCells;
    }


}
