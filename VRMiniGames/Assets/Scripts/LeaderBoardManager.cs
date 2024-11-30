using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] nameTexts; // 이름 텍스트 (8개)
    [SerializeField] private TextMeshProUGUI[] scoreTexts; // 점수 텍스트 (8개)

    private List<(string name, int score)> leaderboard = new List<(string name, int score)>();

    // 새 점수 추가 및 정렬
    public void UpdateLeaderboard(string playerName, int playerScore)
    {
        leaderboard.Add((playerName, playerScore));
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score)); // 내림차순 정렬

        // 상위 8명만 표시
        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (i < leaderboard.Count)
            {
                nameTexts[i].text = leaderboard[i].name;
                scoreTexts[i].text = leaderboard[i].score.ToString();
            }
            else
            {
                nameTexts[i].text = "";
                scoreTexts[i].text = "";
            }
        }
    }
}
