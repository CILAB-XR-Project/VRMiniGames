using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GameSave : MonoBehaviour
{

    //public string playerName;   // Player name
    [SerializeField] private string playerName = "Player1"; // Default value: "Player1"
    public float miniGameScore; // 플레이어 점수

    // Save Score Function
    public void SaveScore(string fileName, bool isObstacle)
    {
        /* How To Use
        // Save In ObstBestScore
        GameManager.Instance.playerName = "Alice"; // 
        GameManager.Instance.miniGameScore = 45.2f;
        GameManager.Instance.SaveScore("ObstBestScore", true); // ObstBestScore에 저장

        // Save In  GRLightBestScore
        GameManager.Instance.playerName = "Bob";
        GameManager.Instance.miniGameScore = 78.9f;
        GameManager.Instance.SaveScore("GRLightBestScore", false); // GRLightBestScore에 저장
         */
        if (miniGameScore <= 0)
        {
            Debug.Log($"Score is zero or less. Skipping save for {playerName}.");
            return;
        }

        // load json file
        TextAsset jsonFile = Resources.Load<TextAsset>($"Score/{fileName}");
        BestScoreData scoreData = new BestScoreData();

        if (jsonFile != null)
        {
            scoreData = JsonUtility.FromJson<BestScoreData>(jsonFile.text);
        }

        // save new score data
        if (isObstacle)
        {   // Obstacle Run
            var scoreList = new List<ScoreEntry>(scoreData.ObstBestScore ?? new ScoreEntry[0]);
            scoreList.Add(new ScoreEntry { Player = playerName, Score = miniGameScore });
            scoreData.ObstBestScore = scoreList.ToArray();
        }
        else
        {
            var scoreList = new List<ScoreEntry>(scoreData.GRLightBestScore ?? new ScoreEntry[0]);
            scoreList.Add(new ScoreEntry { Player = playerName, Score = miniGameScore });
            scoreData.GRLightBestScore = scoreList.ToArray();
        }
        // save as JSON file
        string filePath = Path.Combine(Application.dataPath, $"Resources/Score/{fileName}.json");
        string jsonData = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(filePath, jsonData);

        Debug.Log($"Score saved for {playerName} in {fileName}.json.");
    }

}

