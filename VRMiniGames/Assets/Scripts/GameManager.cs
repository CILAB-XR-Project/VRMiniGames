using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    //[SerializeField] private Text[] playerTexts; // Player1 ~ Player8
    //[SerializeField] private Text[] scoreTexts;  // Score1 ~ Score8
    [SerializeField] private TMP_Text[] obstPlayerTexts; // ObstBestScore�� Player1~Player8
    [SerializeField] private TMP_Text[] obstScoreTexts;  // ObstBestScore�� Score1~Score8
    // �б� ���� ������Ƽ�� ���� �ܺο��� ���� ����
    public TMP_Text[] ObstPlayerTexts => obstPlayerTexts;
    public TMP_Text[] ObstScoreTexts => obstScoreTexts;
    [SerializeField] private TMP_Text[] grlightPlayerTexts; // GRLightBestScore�� Player1~Player8
    [SerializeField] private TMP_Text[] grlightScoreTexts;  // GRLightBestScore�� Score1~Score8
    
    public TMP_Text[] GRLightPlayerTexts => grlightPlayerTexts;
    public TMP_Text[] GRLightScoreTexts => grlightScoreTexts;


    private void Awake()
    {
        // ObstBestScore �ؽ�Ʈ �ڵ� ����
        obstPlayerTexts = FindTexts("ObstPlayer", 8); // "ObstPlayer1" ~ "ObstPlayer8"
        obstScoreTexts = FindTexts("ObstScore", 8);   // "ObstScore1" ~ "ObstScore8"

        // GRLightBestScore �ؽ�Ʈ �ڵ� ����
        grlightPlayerTexts = FindTexts("GRLightPlayer", 8); // "GRLightPlayer1" ~ "GRLightPlayer8"
        grlightScoreTexts = FindTexts("GRLightScore", 8);   // "GRLightScore1" ~ "GRLightScore8"
    }

    // �ؽ�Ʈ ������Ʈ�� �ڵ����� ã�� �迭�� ��ȯ�ϴ� �Լ�
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
                Debug.LogError($"������Ʈ {objectName}�� ã�� �� �����ϴ�!");
            }
        }

        return texts;
    }
    
}
