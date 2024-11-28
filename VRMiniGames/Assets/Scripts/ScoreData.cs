using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string Player;
    public float Score; // 플레이 시간
}

[System.Serializable]
public class BestScoreData
{
    public ScoreEntry[] ObstBestScore;
    public ScoreEntry[] GRLightBestScore;
}
