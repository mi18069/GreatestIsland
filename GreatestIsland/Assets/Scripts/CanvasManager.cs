using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Canvas canvas;

    private void Start()
    {
        // Set the CanvasGroup's alpha to 0 (invisible), but keep it active
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void HideCanvas()
    {
        canvasGroup.alpha = 0f; 
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false;
        canvas.enabled = true;

        Canvas.ForceUpdateCanvases();

    }

    public void ShowCanvas()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true; 
        canvasGroup.blocksRaycasts = true;
        canvas.enabled = false;


        Canvas.ForceUpdateCanvases();

    }
}
