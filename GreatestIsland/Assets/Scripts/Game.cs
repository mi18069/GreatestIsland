using UnityEngine;

public class Game : MonoBehaviour
{
    private Board board;
    private Map map;
    private HttpClient client;
    private CameraManipulation cameraManipulation;

    private Cell currentCell = Cell.InvalidCell;
    private Island currentIsland = Island.InvalidIsland;


    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        client = GetComponent<HttpClient>();
        cameraManipulation = GetComponent<CameraManipulation>();
    }

    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        var matrix = client.GetNewMatrix();
        map = new Map();
        map.CreateMapFromMatrix(matrix);
        board.Draw(map);
        cameraManipulation.AdjustCameraToTilemap(Camera.main, board.cellTilemap);

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
        else if (island.state != Island.State.Default && island.state != Island.State.Selected)
            return;
        else
        {
            if (map.CheckIsland(island))
                Debug.Log("Greatest island found. Good job!");
            else
            {
                StartCoroutine(cameraManipulation.Shake(.2f, .3f));
                Debug.Log("Not the greatest island, try again");

            }
        }


        board.RedrawIsland(island);
    }


}
