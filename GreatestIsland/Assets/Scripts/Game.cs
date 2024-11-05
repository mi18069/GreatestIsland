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
                                    {0, 150, 200, 0, 0 }};

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
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);

        var map = new Map(width, height, 1000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        camera.transform.position = new Vector3(map.width / 2f, map.height / 2f, 10f);
        camera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

}
