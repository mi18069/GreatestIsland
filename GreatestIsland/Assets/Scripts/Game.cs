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
        map = CreateMapFromMatrix(matrix);
        CenterCameraToMap(map);
        board.Draw(map);
    }

    private Map CreateMapFromMatrix(int[,] matrix)
    {
        int height = matrix.GetLength(0);
        int width = matrix.GetLength(1);

        var map = new Map(height, width, 1000);

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                var cell = new Cell(x, y, matrix[x, y]);
                map.AddCellIntoMap(cell);
            }
        }

        return map;
    }

    private void CenterCameraToMap(Map map)
    {
        var camera = Camera.main;
        camera.transform.position = new Vector3(map.height / 2f, map.width / 2f, -10f);
        camera.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickedCell();
        }
    }

    private void ClickedCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = map.GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid)
        {
            return;
        }

        Debug.Log($"Clicked cell ({cell.position.x}, {cell.position.y}): {cell.height}");
        board.RedrawCell(map, cell);
    }


}
