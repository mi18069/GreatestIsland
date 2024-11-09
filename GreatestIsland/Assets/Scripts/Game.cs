using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Board board;
    private Map map;
    private HttpClient client;
    private CameraManipulation cameraManipulation;

    private Cell currentCell = Cell.InvalidCell;
    private Island currentIsland = Island.InvalidIsland;
    private int numOfLives;
    private bool canUserGuess = false;
    private int[,] defaultMatrix = {{0, 0, 240, 245, 180 },
                                    {50, 0, 0, 200, 180 },
                                    {600, 70, 0, 0, 100 },
                                    {500, 0, 0, 0, 30 },
                                    {0, 0, 500, 0, 0 },
                                    {0, 150, 200, 50, 0 },
                                    {100, 0, 100, 0, 0 },
                                    {200, 0, 30, 0, 300 } };

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        client = GetComponent<HttpClient>();
        cameraManipulation = GetComponent<CameraManipulation>();
    }

    private void Start()
    {
        numOfLives = 3;
        NewGame();
    }
    private void NewGame()
    {
        canUserGuess = true;
        var matrix = client.GetNewMatrix();
        //var matrix = defaultMatrix;
        map = new Map();
        map.CreateMapFromMatrix(matrix);
        board.Draw(map);
        cameraManipulation.AdjustCameraToTilemap(Camera.main, board.cellTilemap);

    } 

    private void Update()
    {
        if (!canUserGuess)
            return;
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

        if (currentIsland != newIsland)
        {

            if (currentIsland.state == Island.State.Selected)
            {
                currentIsland.state = Island.State.Default;
                board.RedrawIsland(currentIsland);
            }

            currentIsland = newIsland;

            if (currentIsland.state == Island.State.Missed || currentIsland.state == Island.State.Found)
                return;
            
            if (currentIsland.state != Island.State.Invalid) 
            {
                currentIsland.state = Island.State.Selected;
                board.RedrawIsland(currentIsland);
            }
        }
 

    }

    private void ClickedCell()
    { 
        Island island = currentIsland;

        if (island.state == Island.State.Missed)
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            return;
        }

        if (island.state != Island.State.Default && island.state != Island.State.Selected)
            return;


        bool success = map.CheckIsland(island);
        island.state = success ? Island.State.Found :Island.State.Missed;
        board.RedrawIsland(island);

        if (success)
        {
            Debug.Log("Greatest island found. Good job!");
            canUserGuess = false;
            StartCoroutine(ProceedToNextLevelWithDelay(3));
        }
        else
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            Debug.Log("Not the greatest island, try again");
            numOfLives--;
            if (numOfLives <= 0)
            {
                FinishGame();
            }
        }
        
    }

    private void FinishGame()
    {
        // Shows interface with player stats

    }
    private IEnumerator ProceedToNextLevelWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds); // Waits without freezing the main thread
        NextLevel();
    }
    private void NextLevel()
    {
        NewGame();
    }

}
