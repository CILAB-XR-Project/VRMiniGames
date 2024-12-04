using UnityEngine;
using TMPro;
using System.Linq;
public class DisplayBestScores : MonoBehaviour
{
    private TMP_Text[] playerTexts;
    private TMP_Text[] scoreTexts;

    void Start()
    {
        Transform obstParent = transform.Find("Obst_Run_Board/Obst_Run_UI");
        Transform lightParent = transform.Find("GR_Light_Board/GR_UI");
        if (obstParent == null)
        {
            Debug.LogError("Obst_Run_UI�� ã�� �� �����ϴ�!");
            return;
        }

        if (lightParent == null)
        {
            Debug.LogError("GR_UI�� ã�� �� �����ϴ�!");
            return;
        }
        // ObstBestScore �ؽ�Ʈ �迭 �ʱ�ȭ
        TMP_Text[] obstPlayerTexts = new TMP_Text[8];
        TMP_Text[] obstScoreTexts = new TMP_Text[8];

        for (int i = 0; i < 8; i++)
        {
            obstPlayerTexts[i] = obstParent.Find($"ObstPlayer{i + 1}").GetComponent<TMP_Text>();
            obstScoreTexts[i] = obstParent.Find($"ObstScore{i + 1}").GetComponent<TMP_Text>();
        }

        // GBLightBestScore �ؽ�Ʈ �迭 �ʱ�ȭ
        TMP_Text[] lightPlayerTexts = new TMP_Text[8];
        TMP_Text[] lightScoreTexts = new TMP_Text[8];

        for (int i = 0; i < 8; i++)
        {
            lightPlayerTexts[i] = lightParent.Find($"GRLightPlayer{i + 1}").GetComponent<TMP_Text>();
            lightScoreTexts[i] = lightParent.Find($"GRLightScore{i + 1}").GetComponent<TMP_Text>();
        }

        // JSON ���Ϻ��� �ؽ�Ʈ ������Ʈ
        LoadAndDisplayScores("ObstBestScore", obstPlayerTexts, obstScoreTexts, true);
        LoadAndDisplayScores("GRLightBestScore", lightPlayerTexts, lightScoreTexts, false);
    }

        private void LoadAndDisplayScores(string fileName, TMP_Text[] playerTexts, TMP_Text[] scoreTexts, bool isObstacle)
    {
        // JSON ���� �ε� �� ó��
        TextAsset jsonFile = Resources.Load<TextAsset>($"Score/{fileName}");
        if (jsonFile == null)
        {
            Debug.LogError($"JSON ���� {fileName}�� ã�� �� �����ϴ�!");
            return;
        }

        BestScoreData scoreData = JsonUtility.FromJson<BestScoreData>(jsonFile.text);
        if (scoreData == null)
        {
            Debug.LogError($"JSON �����͸� ��ȯ�� �� �����ϴ�. ���� �̸�: {fileName}");
            return;
        }
        ScoreEntry[] scores = isObstacle ? scoreData.ObstBestScore : scoreData.GRLightBestScore;
        if (scores == null || scores.Length == 0)
        {
            Debug.LogError($"���� �����Ͱ� ��� �ֽ��ϴ�. ���� �̸�: {fileName}, isObstacle: {isObstacle}");
            scores = new ScoreEntry[0]; // �⺻������ �� �迭 ����
        }
        var sortedScores = scores
            .OrderBy(entry => entry.Score)
            .Take(8)
            .ToArray();

        for (int i = 0; i < playerTexts.Length; i++)
        {
            if (i < sortedScores.Length)
            {
                playerTexts[i].text = sortedScores[i].Player;
                scoreTexts[i].text = sortedScores[i].Score.ToString("F2") + "s";
            }
            else
            {
                playerTexts[i].text = "-";
                scoreTexts[i].text = "-";
            }
        }
    }
}
