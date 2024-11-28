using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] nameTexts; // �̸� �ؽ�Ʈ (8��)
    [SerializeField] private TextMeshProUGUI[] scoreTexts; // ���� �ؽ�Ʈ (8��)

    private List<(string name, int score)> leaderboard = new List<(string name, int score)>();

    // �� ���� �߰� �� ����
    public void UpdateLeaderboard(string playerName, int playerScore)
    {
        leaderboard.Add((playerName, playerScore));
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score)); // �������� ����

        // ���� 8�� ǥ��
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
