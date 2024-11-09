using System;
using TMPro;
using UnityEngine;

public class DisplayStats : MonoBehaviour
{
    public TMP_Text statsText;

    void Start() {
        SetUserStats();
    }

    private void SetUserStats()
    {
        int levelsPassed = UserStats.Instance.NumOfLevelsPassed;
        float accuracy = (float)Math.Round(UserStats.Instance.Accuracy, 2);
        int time = UserStats.Instance.ElapsedTime;
        int averageTimePerLevel = UserStats.Instance.AverageTimePerLevel;

        string statsTextContent = $"Levels Passed: {levelsPassed}\nTime: {time} sec\n\nAccuracy: {accuracy.ToString("F2")}%\nAverage time per level: {averageTimePerLevel} sec\n";

        if (statsText != null)
            statsText.text = statsTextContent;

    }
}
