using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

    private System.Random random;
    private string[] startingMessages = {"Good luck!", "Watch for details!", "Have fun", "New game - new opportunity", "I'm cheering for you"};
    private string[] successMessages = {"Good job!", "Nice!", "Wow!", "Well done!", "Excellent"};
    private string[] missMessages = {"Unlucky", "Almost got it", "Whoopsy daisy", "It happens"};
    private string[] cheekyMessages = { "Nope", "Still no", ":(", "...", "NO", "Try again", "Maybe press harder", "Next time a charm", "Speechless", "Shocked smiley face" };
    private string[] endMessages = { "It'll be better next time!", "Well played!", "Good game" };
    private string[] inactiveMessages = { "Bit sleepy?", "It's a though one", "Take your time", "Choose carefully", "Are you there?" };

    public SceneManagerScript sceneManagerScript;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI livesRemaining;
    public TextMeshProUGUI textMessages;
    [SerializeField] ConfirmationWindow confirmationWindow;

    private float time = 0;
    private float inactiveTime = 0;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        client = GetComponent<HttpClient>();
        cameraManipulation = GetComponent<CameraManipulation>();
        sceneManagerScript = GetComponent<SceneManagerScript>();
        random = new System.Random();
    }

    private void Start()
    {
        numOfLives = 3;
        UpdateLives(numOfLives);
        UpdateTime(0);
        UpdateMessage(startingMessages);
        UserStats.Instance.ResetStats();
        NewGame();
    }

    private void UpdateLives(int numOfLives)
    {
        livesRemaining.text = $"Lives: {numOfLives}";
    }

    private void UpdateTime(float time)
    {
        timer.text = RepresentativeTime(time);
    }

    private void UpdateMessage(string[] messages)
    {
        int numOfMessages = messages.Count();        
        textMessages.text = messages[random.Next(numOfMessages)];
    }

    private string RepresentativeTime(float time)
    {
        int timeInt = Mathf.RoundToInt(time);
        return  $"{(timeInt/60).ToString().PadLeft(2, '0')}:{(timeInt%60).ToString().PadLeft(2, '0')}";
    }

    private void UpdateInactiveTime(float deltaTime)
    {
        inactiveTime += deltaTime;
        if (inactiveTime >= 5)
        {
            inactiveTime = 0;
            UpdateMessage(inactiveMessages);
        }
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
        if (Input.GetKeyDown("escape"))
        {
            ShowConfirmationWindow();

        }
        if (!canUserGuess)
            return;

        time += Time.deltaTime;
        UpdateInactiveTime(Time.deltaTime);
        UpdateTime(time);

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

        inactiveTime = 0;

        if (island.state == Island.State.Missed)
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            UpdateMessage(cheekyMessages);
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
            UpdateMessage(successMessages);
            UserStats.Instance.IncrementLevelsPassed();
            board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());
            StartCoroutine(ProceedToNextLevelWithDelay(3));

        }
        else
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            numOfLives--;
            UpdateLives(numOfLives);
            if (numOfLives <= 0)
            {
                canUserGuess = false;
                UpdateMessage(endMessages);
                board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());
                StartCoroutine(ProceedToGameOverScreenWithDelay(3));
            }
            else
            {
                UpdateMessage(missMessages);
            }

        }
        
    }
    private void ShowConfirmationWindow()
    {
        canUserGuess = false;
        confirmationWindow.gameObject.SetActive(true);
        confirmationWindow.yesButton.onClick.AddListener(YesClicked);
        confirmationWindow.noButton.onClick.AddListener(NoClicked);
    }

    private void YesClicked()
    {
        canUserGuess = true;
        confirmationWindow.yesButton.onClick.RemoveListener(YesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(NoClicked);
        confirmationWindow.gameObject.SetActive(false);
        GameOver();

    }

    private void NoClicked()
    {
        canUserGuess = true;
        confirmationWindow.yesButton.onClick.RemoveListener(YesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(NoClicked);
        confirmationWindow.gameObject.SetActive(false);

    }



    private IEnumerator ProceedToGameOverScreenWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        GameOver();
    }

    private void GameOver()
    {
        UserStats.Instance.SetElapsedTime(Mathf.RoundToInt(time));
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
