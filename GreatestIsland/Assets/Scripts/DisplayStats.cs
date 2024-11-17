using System;
using TMPro;
using UnityEngine;

public class DisplayStats : MonoBehaviour
{
    public TMP_Text statsText;


    void Start() {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBackground(AudioManager.instance.backgroundMenu);
        }
        SetUserStats();
    }

    private void SetUserStats()
    {
        int levelsPassed = UserStats.Instance.NumOfLevelsPassed;
        float accuracy = (float)Math.Round(UserStats.Instance.Accuracy, 2);
        int time = UserStats.Instance.ElapsedTime;
        string representativeTime = GetRepresentativeTime(time);
        int averageTimePerLevel = UserStats.Instance.AverageTimePerLevel;

        string statsTextContent = $"Levels Passed: {levelsPassed}\nTime: {representativeTime} \n\nAccuracy: {accuracy.ToString("F2")}%\nAverage time per level: {averageTimePerLevel} sec\n";

        if (statsText != null)
            statsText.text = statsTextContent;

    }
    private string GetRepresentativeTime(int time)
    {
        int timeInt = Mathf.RoundToInt(time);
        return $"{(timeInt / 60).ToString().PadLeft(2, '0')}:{(timeInt % 60).ToString().PadLeft(2, '0')}";
    }
}
