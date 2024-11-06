using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Board board;
    private Map map;

    private int[,] defaultMatrix = {{0, 0, 240, 245, 180 },
                                    {50, 0, 0, 200, 180 },
                                    {600, 70, 0, 0, 100 },
                                    {500, 0, 0, 0, 30 },
                                    {0, 0, 500, 0, 0 },
                                    {0, 150, 200, 50, 0 },
                                    {100, 0, 100, 0, 0 },
                                    {200, 0, 30, 0, 300 } } ;

    private Cell currentCell = Cell.InvalidCell;
    private Island currentIsland = Island.InvalidIsland;


    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        var matrix = defaultMatrix;
        map = new Map();
        map.CreateMapFromMatrix(matrix);
        CenterCameraToMap(map);
        board.Draw(map);

    }



    private void CenterCameraToMap(Map map)
    {
        var camera = Camera.main;
        camera.transform.position = new Vector3(map.height / 2f, map.width / 2f, -10f);
        camera.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private void Update()
    {
        TrackMouseHovering();
        if (Input.GetMouseButtonDown(0))
        {
            ClickedCell();
        }
    }

    private void TrackMouseHovering()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.cellTilemap.WorldToCell(worldPosition);
        var cell = map.GetCell(cellPosition.x, cellPosition.y);
        if (currentCell != cell)
        {
            HandleChangedCellHover(cell);
        }
    }

    private void HandleChangedCellHover(Cell cell)
    {
        currentCell = cell;
        var newIsland = map.GetCellIsland(currentCell);

        if (newIsland.state == Island.State.Missed || newIsland.state == Island.State.Found)
            return;

        if (currentIsland != newIsland)
        {
            if (currentIsland.state == Island.State.Selected)
            {
                currentIsland.state = Island.State.Default;
                board.RedrawIsland(currentIsland);
            }

            currentIsland = newIsland;
            
            if (currentIsland.state != Island.State.Invalid) 
            {
                currentIsland.state = Island.State.Selected;
                board.RedrawIsland(currentIsland);
            }
        }
 

    }

    private void ClickedCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.cellTilemap.WorldToCell(worldPosition);
        Cell cell = map.GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.type == Cell.Type.Water)
        {
            return;
        }

        Island island = map.GetCellIsland(cell);
        if (island.state != Island.State.Default && island.state != Island.State.Selected)
            return;

        if (map.CheckIsland(island))
            Debug.Log("Greatest island found. Good job!");
        else
            Debug.Log("Not the greatest island, try again");

        board.RedrawIsland(island);
    }


}
