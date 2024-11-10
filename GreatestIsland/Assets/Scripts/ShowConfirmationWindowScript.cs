using UnityEngine;

// For showing confirmation window and exiting game
public class ShowConfirmationWindowScript : MonoBehaviour
{
    [SerializeField] ConfirmationWindow confirmationWindow;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowConfirmationWindow("Are you sure you want to exit game?");
        }
    }
    private void ShowConfirmationWindow(string message)
    {
        confirmationWindow.gameObject.SetActive(true);
        confirmationWindow.messageText.text = message;
        confirmationWindow.yesButton.onClick.AddListener(YesClicked);
        confirmationWindow.noButton.onClick.AddListener(NoClicked);
    }

    private void YesClicked()
    {
        confirmationWindow.yesButton.onClick.RemoveListener(YesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(NoClicked);
        confirmationWindow.gameObject.SetActive(false);
        Application.Quit();
    }

    private void NoClicked()
    {
        confirmationWindow.yesButton.onClick.RemoveListener(YesClicked);
        confirmationWindow.noButton.onClick.RemoveListener(NoClicked);
        confirmationWindow.gameObject.SetActive(false);

    }
}
