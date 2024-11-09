public class UserStats
{
    private static UserStats _instance;

    public static UserStats Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UserStats();
            }
            return _instance;
        }
    }

    public void ResetStats()
    {
        NumOfTries = 0;
        NumOfLevelsPassed = 0;
        ElapsedTime = 0;
    }

    public int NumOfTries { get; private set; } = 0;
    public int NumOfLevelsPassed { get; private set; } = 0;
    public int ElapsedTime { get; private set; } = 0; 

    // Because of 3 seconds wait on level end, that should be in calculations
    public int AverageTimePerLevel
    => NumOfLevelsPassed > 0 ? ElapsedTime / NumOfLevelsPassed : ElapsedTime;

    public float Accuracy
    => NumOfTries > 0 ? (float)NumOfLevelsPassed / NumOfTries : 0.0f;

    public void IncrementTries()
    => NumOfTries++;
    
    public void IncrementLevelsPassed()
    => NumOfLevelsPassed++; 

    public void SetElapsedTime(int time)
    => ElapsedTime = time;
    

}
