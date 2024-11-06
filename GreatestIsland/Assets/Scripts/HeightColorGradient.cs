using UnityEngine;

public class HeightColorGradient : MonoBehaviour
{
    public Gradient fieldGradient;
    public Gradient hillGradient;
    public Gradient mountainGradient;
    public Gradient snowyMountainGradient;

    private Gradient currentGradient;
    private float currentTreshold;

    float fieldTreshold = 0.2f;
    float hillTreshold = 0.5f;
    float mountainTreshold = 0.8f;
    float snowyMountainTreshold = 1.0f;

    private int maxHeight = 1000;

    public Color GetTileColor(Cell cell)
    {
        if (cell.type == Cell.Type.Water)
            return new Color(0.251f, 0.920f, 0.929f);

        float normalizedHeight = Mathf.Clamp01(cell.height / (float)maxHeight);

        // Choose gradient based on normalized height
        if (normalizedHeight < fieldTreshold)
        {
            currentGradient = fieldGradient;
            currentTreshold = fieldTreshold;
        }
        else if (normalizedHeight < hillTreshold)
        {
            currentGradient = hillGradient;
            currentTreshold = hillTreshold;
        }
        else if (normalizedHeight < mountainTreshold)
        {
            currentGradient = mountainGradient;
            currentTreshold = mountainTreshold;
        }
        else
        {
            currentGradient = snowyMountainGradient;
            currentTreshold = snowyMountainTreshold;
        }

        return currentGradient.Evaluate(normalizedHeight / currentTreshold);
    }
}
