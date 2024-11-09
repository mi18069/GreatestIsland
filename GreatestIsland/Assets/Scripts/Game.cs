using System.Collections;
using System.Diagnostics;
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

    public SceneManagerScript sceneManagerScript;
    private Stopwatch stopwatch;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        client = GetComponent<HttpClient>();
        cameraManipulation = GetComponent<CameraManipulation>();
        sceneManagerScript = GetComponent<SceneManagerScript>();
        stopwatch = new Stopwatch();
    }

    private void Start()
    {
        numOfLives = 3;
        UserStats.Instance.ResetStats();
        stopwatch.Start();
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
        if (currentCell.type != cell.type)
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

        UserStats.Instance.IncrementTries();

        if (success)
        {
            canUserGuess = false;
            UserStats.Instance.IncrementLevelsPassed();
            StartCoroutine(ProceedToNextLevelWithDelay(3));

        }
        else
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            numOfLives--;
            if (numOfLives <= 0)
            {
                canUserGuess = false;
                stopwatch.Stop();
                UserStats.Instance.SetElapsedTime((int)(stopwatch.ElapsedMilliseconds / 1000));
                StartCoroutine(ProceedToGameOverScreenWithDelay(3));
            }
        }
        
    }



    private IEnumerator ProceedToGameOverScreenWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        GameOver();
    }

    private void GameOver()
    {
        sceneManagerScript.LoadScene("GameOverScene");

    }

    private IEnumerator ProceedToNextLevelWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        NextLevel();
    }
    private void NextLevel()
    {
        NewGame();
    }

}
