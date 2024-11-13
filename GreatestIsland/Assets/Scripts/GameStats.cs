using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// This class represents layer of information while user plays the game
// It contains information about lives remaining, elapsed time and additional messages at the bottom of the screen
public class GameStats : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesRemainingText;
    public TextMeshProUGUI messagesText;
    public TextMeshProUGUI countdownText;

    private float inactiveTime = 0;

    #region messages for interaction with user
    public enum MessageType
    {
        Start,
        Success,
        Miss,
        Cheeky,
        End,
        Inactive
    }

    private System.Random random;
    private Dictionary<MessageType, string[]> messages = new Dictionary<MessageType, string[]>();
    #endregion

    private void Awake()
    {
        random = new System.Random();
        messages[MessageType.Start] = new string[] { "Good luck!", "Watch for details!", "Have fun", "New game - new opportunity", "I'm cheering for you" };
        messages[MessageType.Success] = new string[] { "Good job!", "Nice!", "Wow!", "Well done!", "Excellent" };
        messages[MessageType.Miss] = new string[] { "Unlucky", "Almost got it", "Whoopsy daisy", "It happens" };
        messages[MessageType.Cheeky] = new string[] { "Nope", "Still no", ":(", "...", "NO", "Try again", "Maybe press harder", "Next time a charm", "Speechless", "Shocked smiley face" };
        messages[MessageType.End] = new string[] { "It'll be better next time!", "Well played!", "Good game" };
        messages[MessageType.Inactive] = new string[] { "Bit sleepy?", "It's a though one", "Take your time", "Choose carefully", "Are you there?" };
    }

    public void ResetInterfaceValues(int numOfLives)
    {
        UpdateLivesText(numOfLives);
        UpdateTimeText(0);
    }

    public void ResetInactiveTime()
    {
        inactiveTime = 0;
    }


    public void UpdateLivesText(int numOfLives)
    {
        livesRemainingText.text = $"Lives: {numOfLives}";
    }

    public void UpdateTimeText(float time)
    {
        timerText.text = GetRepresentativeTime(time);
    }

    public void UpdateCountdownText(float time)
    {
        countdownText.text = Mathf.CeilToInt(time).ToString();
    }

    public void UpdateMessageText(MessageType type)
    {
        ResetInactiveTime();
        string[] messageArray = messages[type];
        int numOfMessages = messageArray.Count();
        messagesText.text = messageArray[random.Next(numOfMessages)];
    }

    public void UpdateTime(float time, float deltaTime)
    {
        UpdateTimeText(time);
        UpdateInactiveTime(Time.deltaTime);
    }

    private void UpdateInactiveTime(float deltaTime)
    {
        inactiveTime += deltaTime;
        if (inactiveTime >= 5)
        {
            inactiveTime = 0;
            UpdateMessageText(MessageType.Inactive);
        }
    }

    private string GetRepresentativeTime(float time)
    {
        int timeInt = Mathf.RoundToInt(time);
        return $"{(timeInt / 60).ToString().PadLeft(2, '0')}:{(timeInt % 60).ToString().PadLeft(2, '0')}";
    }
}
