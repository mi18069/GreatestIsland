using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum GameMode
    {
        Normal,
        Fog,
        TimeRush
    }

    public static GameMode selectedMode { get; set; }

    private Board board;
    private Map map;
    private HttpClient client;
    private CameraManipulation cameraManipulation;

    private Cell currentCell = Cell.InvalidCell;
    private Island currentIsland = Island.InvalidIsland;

    private int numOfLives;
    private bool canUserGuess = false;
    private bool hasGameStarted = false;

    public GameStats gameStats;
    public SceneManagerScript sceneManagerScript;

    [SerializeField] ConfirmationWindow confirmationWindow;
    [SerializeField] ConfirmationWindow errorWindow;

    AudioManager audioManager;

    private float time = 0;
    private float countdownTime = 10;
    private float timeRemaining = 10;
    private bool freezeCountdown = true;

    #region Unity functions
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        client = GetComponent<HttpClient>();
        cameraManipulation = GetComponent<CameraManipulation>();
        audioManager = AudioManager.instance;
    }

    private void Start()
    {
        ResetUserStats();
        gameStats.ResetInterfaceValues(numOfLives);
        audioManager.PlayBackground(audioManager.backgroundGame);
        NewGame();
    }

    private void Update()
    {

        if (!hasGameStarted)
            return;

        UpdateTime(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowConfirmationWindow("Are you sure you want to finish?");

        }


        if (!canUserGuess)
            return;

        TrackMouseHovering();
        if (Input.GetMouseButtonDown(0))
        {
            ClickedCell();
        }
    }

    #endregion

    #region User interaction
    private void TrackMouseHovering()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.cellTilemap.WorldToCell(worldPosition);
        var cell = map.GetCell(cellPosition.x, cellPosition.y);
        if (currentCell.position != cell.position)
        {
            HandleChangedCellHover(cell);

            if (selectedMode == GameMode.Fog)
                board.FogCenterCellChanged(cell, map);
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

        if (island.state == Island.State.Invalid)
            return;

        // User still interacts with environment, but it doesn't count as new attempt
        if (island.state == Island.State.Missed)
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            gameStats.UpdateMessageText(GameStats.MessageType.Cheeky);
            audioManager.PlaySFX(audioManager.islandMiss);
            return;
        }

        // Cannot interact 
        if (island.state != Island.State.Default && island.state != Island.State.Selected)
            return;


        // Check guess
        UserStats.Instance.IncrementTries();

        bool success = map.CheckIsland(island);
        island.state = success ? Island.State.Found :Island.State.Missed;
        board.RedrawIsland(island);

        if (success)
        {
            UserGuessedCorrectly();
        }
        else
        {
            UserGuessedIncorrectly();
        }
        
    }

    #endregion

    #region Confirmation Window
    private void ShowConfirmationWindow(string message)
    {
        canUserGuess = false;
        confirmationWindow.gameObject.SetActive(true);
        confirmationWindow.messageText.text = message;
        confirmationWindow.yesButton.onClick.AddListener(ConfirmationYesClicked);
        confirmationWindow.noButton.onClick.AddListener(ConfirmationNoClicked);
    }

    private void ConfirmationYesClicked()
    {
        canUserGuess = true;
        confirmationWindow.yesButton.onClick.RemoveListener(ConfirmationYesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(ConfirmationNoClicked);
        confirmationWindow.gameObject.SetActive(false);
        GameOver();

    }

    private void ConfirmationNoClicked()
    {
        canUserGuess = true;
        confirmationWindow.yesButton.onClick.RemoveListener(ConfirmationYesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(ConfirmationNoClicked);
        confirmationWindow.gameObject.SetActive(false);

    }

    #endregion

    #region Error window

    private void ShowErrorWindow(string message)
    {
        canUserGuess = false;
        errorWindow.gameObject.SetActive(true);
        errorWindow.messageText.text = message;
        errorWindow.yesButton.onClick.AddListener(ErrorYesClicked);
        errorWindow.noButton.onClick.AddListener(ErrorNoClicked);
    }

    private void ErrorYesClicked()
    {
        canUserGuess = true;
        errorWindow.yesButton.onClick.RemoveListener(ErrorYesClicked);
        errorWindow.noButton.onClick.RemoveListener(ErrorNoClicked);
        errorWindow.gameObject.SetActive(false);
        NewGame();
    }

    private void ErrorNoClicked()
    {
        canUserGuess = true;
        errorWindow.yesButton.onClick.RemoveListener(ErrorYesClicked);
        errorWindow.noButton.onClick.RemoveListener(ErrorNoClicked);
        errorWindow.gameObject.SetActive(false);
        GameOver();
    }

    #endregion

    #region Game workflow

    private void NewGame()
    {
        hasGameStarted = false;

        var matrix = client.GetNewMatrix();
        if (matrix.Length < 1)
        {
            ShowErrorWindow("Map cannot be loaded, try again?");
            return;
        }
        map = new Map();
        map.CreateMapFromMatrix(matrix);
        board.Draw(map);

        if (selectedMode == GameMode.Fog)
            board.DrawFog(map);

        cameraManipulation.AdjustCameraToTilemap(Camera.main, board.cellTilemap);
        audioManager.PlaySFX(audioManager.newGame);

        gameStats.UpdateMessageText(GameStats.MessageType.Start);
        if (selectedMode == GameMode.TimeRush)
            ResetCountdownTime();

        freezeCountdown = false;
        canUserGuess = true;
        hasGameStarted = true;
    }

    private void ResetCountdownTime()
    {
        timeRemaining = countdownTime;
    }

    private void ResetUserStats()
    {
        numOfLives = 3;
        UserStats.Instance.ResetStats();
    }

    private void UpdateTime(float deltaTime)
    {
        time += deltaTime;
        gameStats.UpdateTime(time, deltaTime);

        if (selectedMode == GameMode.TimeRush && !freezeCountdown)
        {
            int previousInt = Mathf.RoundToInt(timeRemaining);
            timeRemaining -= deltaTime;
            int newInt = Mathf.RoundToInt(timeRemaining);
            if (previousInt != newInt)
            {
                gameStats.UpdateCountdownText(Math.Max(newInt, 0));
                audioManager.PlaySFX(audioManager.timeTick);
                if (timeRemaining <= 0)
                {
                    UserLost();
                }
            }

        }
    }

    private void UserGuessedCorrectly()
    {
        canUserGuess = false;
        freezeCountdown = true;
        gameStats.UpdateMessageText(GameStats.MessageType.Success);
        UserStats.Instance.IncrementLevelsPassed();
        audioManager.PlaySFX(audioManager.islandSuccess);

        if (selectedMode == GameMode.Fog)
            board.HideFog();
        board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());
        StartCoroutine(ProceedToNextLevelWithDelay(3));
    }

    private void UserGuessedIncorrectly()
    {
        StartCoroutine(cameraManipulation.Shake(.2f, .3f));
        numOfLives--;
        gameStats.UpdateLivesText(numOfLives);
        if (numOfLives <= 0)
        {
            audioManager.PlaySFX(audioManager.endGame);
            UserLost();           
        }
        else
        {
            audioManager.PlaySFX(audioManager.islandMiss);
            gameStats.UpdateMessageText(GameStats.MessageType.Miss);
        }
    }

    private void ShowTargetIslands()
    {
        var targetIslands = map.GetTargetIslands();
        foreach (var island in targetIslands)
        {
            island.state = Island.State.Found;
            board.RedrawIsland(island);
        }
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

    private void UserLost()
    {
        canUserGuess = false;
        hasGameStarted = false;
        freezeCountdown = true;

        gameStats.UpdateMessageText(GameStats.MessageType.End);
        ShowTargetIslands();
        board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());

        if (selectedMode == GameMode.Fog)
            board.HideFog();
        StartCoroutine(ProceedToGameOverScreenWithDelay(3));
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

    #endregion

}
