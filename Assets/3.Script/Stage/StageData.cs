public enum GoalType
{
    ClearCount,    // 총 N개 제거
    ClearColor,    // 특정 색 N개 제거
    Score,         // N점 달성
}

public enum PuzzleType
{
    None = 0,
    Cherry,
    Orange,
    Banana,
    BlueBerry,
    Melon,
    Grape,
}

[System.Serializable]
public class StageData
{
    public int stageIndex;
    public Goal goal;
    public int[] activeCells; // 길이 81 고정
}

[System.Serializable]
public class Goal
{
    public GoalType goalType;
    public PuzzleType puzzleType;
    public int count;
}