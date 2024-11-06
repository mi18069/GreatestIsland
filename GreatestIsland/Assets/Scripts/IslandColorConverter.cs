using UnityEngine;

public class IslandColorConverter : MonoBehaviour
{
    public Color Convert(Island island)
    {

        if (island.state == Island.State.Default)
        {
            return new Color(0, 0, 0, 0); // transparent
        }
        else if (island.state == Island.State.Missed)
        {
            return new Color(0.5f, 0, 0, 0.9f); // darken
        }
        else if (island.state == Island.State.Selected)
        {
            return new Color(0, 0, 0.5f, 0.9f); // light blue
        }
        else if (island.state == Island.State.Found)
        {
            return new Color(0, 0.5f, 0, 0.9f); // light green
        }
        else
        {
            return new Color(0, 0, 0, 0); // transparent
        }
    }
}
