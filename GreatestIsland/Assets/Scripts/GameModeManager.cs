using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    public SceneManagerScript sceneManagerScript;

    public void SetGameMode(string gameMode)
    {
        if (string.Compare(gameMode, "normal") == 0)
        {
            Game.selectedMode = Game.GameMode.Normal;
            sceneManagerScript.LoadScene("GreatestIslandScene");
        }
        else if (string.Compare(gameMode, "fog") == 0)
        {
            Game.selectedMode = Game.GameMode.Fog;
            sceneManagerScript.LoadScene("GreatestIslandScene");

        }
        else if (string.Compare(gameMode, "timeRush") == 0)
        {
            Game.selectedMode = Game.GameMode.TimeRush;
            sceneManagerScript.LoadScene("GreatestIslandScene");

        }
        else 
        {
            Debug.Log("Unknown game mode");
        }
    }


}
