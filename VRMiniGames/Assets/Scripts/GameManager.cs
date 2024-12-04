using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TMP_Text[] obstPlayerTexts; // ObstBestScoreÀÇ Player1~Player8
    [SerializeField] private TMP_Text[] obstScoreTexts;  // ObstBestScoreÀÇ Score1~Score8
                                                          
    // Externally accessible through read-only properties
    public TMP_Text[] ObstPlayerTexts => obstPlayerTexts;
    public TMP_Text[] ObstScoreTexts => obstScoreTexts;
    [SerializeField] private TMP_Text[] grlightPlayerTexts; // GRLightBestScore Player1~Player8
    [SerializeField] private TMP_Text[] grlightScoreTexts;  // GRLightBestScore Score1~Score8
    
    public TMP_Text[] GRLightPlayerTexts => grlightPlayerTexts;
    public TMP_Text[] GRLightScoreTexts => grlightScoreTexts;
    [SerializeField] private string player = "Player1"; // Default value: "Player1"

    // Public getter and setter for player nickname
    public string Player
    {
        get => player;
        set => player = value;
    }

    /* How to use in Other Scene
         void Start()
    {
        string playerName = GameManager.Instance.player;
        Debug.Log("Current Player Name: " + playerName);
    }
     */
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
    
}
