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
            Debug.LogError("Obst_Run_UI를 찾을 수 없습니다!");
            return;
        }

        if (lightParent == null)
        {
            Debug.LogError("GR_UI를 찾을 수 없습니다!");
            return;
        }
        // ObstBestScore 텍스트 배열 초기화
        TMP_Text[] obstPlayerTexts = new TMP_Text[8];
        TMP_Text[] obstScoreTexts = new TMP_Text[8];

        for (int i = 0; i < 8; i++)
        {
            obstPlayerTexts[i] = obstParent.Find($"ObstPlayer{i + 1}").GetComponent<TMP_Text>();
            obstScoreTexts[i] = obstParent.Find($"ObstScore{i + 1}").GetComponent<TMP_Text>();
        }

        // GBLightBestScore 텍스트 배열 초기화
        TMP_Text[] lightPlayerTexts = new TMP_Text[8];
        TMP_Text[] lightScoreTexts = new TMP_Text[8];

        for (int i = 0; i < 8; i++)
        {
            lightPlayerTexts[i] = lightParent.Find($"GRLightPlayer{i + 1}").GetComponent<TMP_Text>();
            lightScoreTexts[i] = lightParent.Find($"GRLightScore{i + 1}").GetComponent<TMP_Text>();
        }

        // JSON 파일별로 텍스트 업데이트
        LoadAndDisplayScores("ObstBestScore", obstPlayerTexts, obstScoreTexts, true);
        LoadAndDisplayScores("GRLightBestScore", lightPlayerTexts, lightScoreTexts, false);
    }

        private void LoadAndDisplayScores(string fileName, TMP_Text[] playerTexts, TMP_Text[] scoreTexts, bool isObstacle)
    {
        // JSON 파일 로드 및 처리
        TextAsset jsonFile = Resources.Load<TextAsset>($"Score/{fileName}");
        if (jsonFile == null)
        {
            Debug.LogError($"JSON 파일 {fileName}을 찾을 수 없습니다!");
            return;
        }

        BestScoreData scoreData = JsonUtility.FromJson<BestScoreData>(jsonFile.text);
        if (scoreData == null)
        {
            Debug.LogError($"JSON 데이터를 변환할 수 없습니다. 파일 이름: {fileName}");
            return;
        }
        ScoreEntry[] scores = isObstacle ? scoreData.ObstBestScore : scoreData.GRLightBestScore;
        if (scores == null || scores.Length == 0)
        {
            Debug.LogError($"점수 데이터가 비어 있습니다. 파일 이름: {fileName}, isObstacle: {isObstacle}");
            scores = new ScoreEntry[0]; // 기본값으로 빈 배열 설정
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
