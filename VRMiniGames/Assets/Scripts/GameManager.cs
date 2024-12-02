using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    //[SerializeField] private Text[] playerTexts; // Player1 ~ Player8
    //[SerializeField] private Text[] scoreTexts;  // Score1 ~ Score8
    [SerializeField] private TMP_Text[] obstPlayerTexts; // ObstBestScore의 Player1~Player8
    [SerializeField] private TMP_Text[] obstScoreTexts;  // ObstBestScore의 Score1~Score8
    // 읽기 전용 프로퍼티를 통해 외부에서 접근 가능
    public TMP_Text[] ObstPlayerTexts => obstPlayerTexts;
    public TMP_Text[] ObstScoreTexts => obstScoreTexts;
    [SerializeField] private TMP_Text[] grlightPlayerTexts; // GRLightBestScore의 Player1~Player8
    [SerializeField] private TMP_Text[] grlightScoreTexts;  // GRLightBestScore의 Score1~Score8
    
    public TMP_Text[] GRLightPlayerTexts => grlightPlayerTexts;
    public TMP_Text[] GRLightScoreTexts => grlightScoreTexts;


    private void Awake()
    {
        // ObstBestScore 텍스트 자동 연결
        obstPlayerTexts = FindTexts("ObstPlayer", 8); // "ObstPlayer1" ~ "ObstPlayer8"
        obstScoreTexts = FindTexts("ObstScore", 8);   // "ObstScore1" ~ "ObstScore8"

        // GRLightBestScore 텍스트 자동 연결
        grlightPlayerTexts = FindTexts("GRLightPlayer", 8); // "GRLightPlayer1" ~ "GRLightPlayer8"
        grlightScoreTexts = FindTexts("GRLightScore", 8);   // "GRLightScore1" ~ "GRLightScore8"
    }

    // 텍스트 오브젝트를 자동으로 찾아 배열로 반환하는 함수
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
                Debug.LogError($"오브젝트 {objectName}를 찾을 수 없습니다!");
            }
        }

        return texts;
    }
    
}
