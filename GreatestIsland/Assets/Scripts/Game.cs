using System.Collections;
using System.Linq;
using TMPro;
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

    #region messages for interaction with user
    private System.Random random;
    private string[] startingMessages = {"Good luck!", "Watch for details!", "Have fun", "New game - new opportunity", "I'm cheering for you"};
    private string[] successMessages = {"Good job!", "Nice!", "Wow!", "Well done!", "Excellent"};
    private string[] missMessages = {"Unlucky", "Almost got it", "Whoopsy daisy", "It happens"};
    private string[] cheekyMessages = { "Nope", "Still no", ":(", "...", "NO", "Try again", "Maybe press harder", "Next time a charm", "Speechless", "Shocked smiley face" };
    private string[] endMessages = { "It'll be better next time!", "Well played!", "Good game" };
    private string[] inactiveMessages = { "Bit sleepy?", "It's a though one", "Take your time", "Choose carefully", "Are you there?" };
    #endregion

    public SceneManagerScript sceneManagerScript;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI livesRemaining;
    public TextMeshProUGUI textMessages;
    [SerializeField] ConfirmationWindow confirmationWindow;

    private float time = 0;
    private float inactiveTime = 0;

    #region Unity functions
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
        ResetUserStats();
        ResetInterfaceValues();
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowConfirmationWindow("Are you sure you want to finish?");

        }
        if (!canUserGuess)
            return;

        UpdateTime(Time.deltaTime);


        TrackMouseHovering();
        if (Input.GetMouseButtonDown(0))
        {
            ClickedCell();
        }
    }

    #endregion

    #region UI functions
    private void ResetInterfaceValues()
    {
        UpdateLivesText(numOfLives);
        UpdateTimeText(0);
        UpdateMessageText(startingMessages);
    }


    private void UpdateLivesText(int numOfLives)
    {
        livesRemaining.text = $"Lives: {numOfLives}";
    }

    private void UpdateTimeText(float time)
    {
        timer.text = GetRepresentativeTime(time);
    }

    private void UpdateMessageText(string[] messages)
    {
        int numOfMessages = messages.Count();        
        textMessages.text = messages[random.Next(numOfMessages)];
    }

    private string GetRepresentativeTime(float time)
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
            UpdateMessageText(inactiveMessages);
        }
    }   

    private void UpdateTime(float deltaTime)
    {
        time += deltaTime;
        UpdateTimeText(time);
        UpdateInactiveTime(Time.deltaTime);
    }

    #endregion

    #region User interaction
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

        // User still interacts with environment, but it doesn't count as new attempt
        if (island.state == Island.State.Missed)
        {
            StartCoroutine(cameraManipulation.Shake(.2f, .3f));
            UpdateMessageText(cheekyMessages);
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

    #endregion

    #region Game workflow

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

    private void ResetUserStats()
    {
        numOfLives = 3;
        UserStats.Instance.ResetStats();
    }

    private void UserGuessedCorrectly()
    {
        canUserGuess = false;
        UpdateMessageText(successMessages);
        UserStats.Instance.IncrementLevelsPassed();
        board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());
        StartCoroutine(ProceedToNextLevelWithDelay(3));
    }

    private void UserGuessedIncorrectly()
    {
        StartCoroutine(cameraManipulation.Shake(.2f, .3f));
        numOfLives--;
        UpdateLivesText(numOfLives);
        if (numOfLives <= 0)
        {
            canUserGuess = false;
            UpdateMessageText(endMessages);
            board.ShowIslandsAverageHeight(map.GetAllIslands().ToList());
            StartCoroutine(ProceedToGameOverScreenWithDelay(3));
        }
        else
        {
            UpdateMessageText(missMessages);
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
