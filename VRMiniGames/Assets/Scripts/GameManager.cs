using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //public string playerName;   // Player name
    [SerializeField] private string playerName = "Player1"; // Default value: "Player1"
    public float miniGameScore; // �÷��̾� ����

    // text ui for display
    [SerializeField] private TMP_Text[] obstPlayerTexts; // ObstBestScore�� Player1~Player8
    [SerializeField] private TMP_Text[] obstScoreTexts;  // ObstBestScore�� Score1~Score8
                                                          
    // Externally accessible through read-only properties
    public TMP_Text[] ObstPlayerTexts => obstPlayerTexts;
    public TMP_Text[] ObstScoreTexts => obstScoreTexts;
    [SerializeField] private TMP_Text[] grlightPlayerTexts; // GRLightBestScore Player1~Player8
    [SerializeField] private TMP_Text[] grlightScoreTexts;  // GRLightBestScore Score1~Score8
    
    public TMP_Text[] GRLightPlayerTexts => grlightPlayerTexts;
    public TMP_Text[] GRLightScoreTexts => grlightScoreTexts;
    //[SerializeField] private string playerName = "Player1"; // Default value: "Player1"
    public AudioSource audio_lobby;
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (audio_lobby != null)
        {
            audio_lobby.Play(); // Play the audio
        }
        // ObstBestScore 
        obstPlayerTexts = FindTexts("ObstPlayer", 8); // "ObstPlayer1" ~ "ObstPlayer8"
        obstScoreTexts = FindTexts("ObstScore", 8);   // "ObstScore1" ~ "ObstScore8"

        // GRLightBestScore 
        grlightPlayerTexts = FindTexts("GRLightPlayer", 8); // "GRLightPlayer1" ~ "GRLightPlayer8"
        grlightScoreTexts = FindTexts("GRLightScore", 8);   // "GRLightScore1" ~ "GRLightScore8"
    }

    private TMP_Text[] FindTexts(string baseName, int count)
    {
        TMP_Text[] texts = new TMP_Text[count];

        for (int i = 0; i < count; i++)
        {
            string objectName = $"{baseName}{i + 1}"; // e.g., "ObstPlayer1", "ObstScore1"
            GameObject obj = GameObject.Find(objectName);

            if (obj != null)
            {
                texts[i] = obj.GetComponent<TMP_Text>();
            }
            else
            {
                Debug.Log("findText");
                Debug.LogError($"Can't not fine {objectName}!");
            }
        }

        return texts;
    }

    // Save Score Function
    public void SaveScore(string fileName, bool isObstacle)
    {
        /* How To Use
        // Save In ObstBestScore
        GameManager.Instance.playerName = "Alice"; // 
        GameManager.Instance.miniGameScore = 45.2f;
        GameManager.Instance.SaveScore("ObstBestScore", true); // ObstBestScore�� ����

        // Save In  GRLightBestScore
        GameManager.Instance.playerName = "Bob";
        GameManager.Instance.miniGameScore = 78.9f;
        GameManager.Instance.SaveScore("GRLightBestScore", false); // GRLightBestScore�� ����
         */
        if (miniGameScore <= 0)
        {
            Debug.Log($"Score is zero or less. Skipping save for {playerName}.");
            return;
        }
        // save as JSON file
        // load json file
        TextAsset jsonFile = Resources.Load<TextAsset>($"Score/{fileName}");
        BestScoreData scoreData = new BestScoreData();
        //if (File.Exists(filePath))
        //{
        //    string jsonData = File.ReadAllText(filePath); // JSON ���� �б�
        //    scoreData = JsonUtility.FromJson<BestScoreData>(jsonData);
        //}
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
        //string updatedJsonData = JsonUtility.ToJson(scoreData, true);
        //File.WriteAllText(filePath, updatedJsonData);
        string filePath = Path.Combine(Application.dataPath, $"Resources/Score/{fileName}.json");
        string jsonData = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(filePath, jsonData);
        // Refresh 
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif

        Debug.Log($"Score saved for {playerName} in {fileName}.json.");
        
    }
    public void StopLobbyMusic()
    {
        if (audio_lobby != null && audio_lobby.isPlaying)
        {
            audio_lobby.Stop();
        }
    }

}
