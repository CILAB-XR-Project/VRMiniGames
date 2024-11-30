using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    //private void OnTriggerEnter(Collider other)
    //{
    //    // �÷��̾�� �浹�ߴ��� Ȯ��
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("bumped");
    //        // ��Ż �̸��� ���� �� �ε�
    //        if (gameObject.name == "Obst_Run_Portal")
    //        {
    //            SceneManager.LoadScene("Obstacle_run"); // "ObstacleRunScene" ������ �̵�
    //        }
    //        else if (gameObject.name == "GR_Light_Portal")
    //        {
    //            SceneManager.LoadScene("Red_Green_light"); // "LightPuzzleScene" ������ �̵�
    //        }
    //    }
    //}
    public string loadingSceneName = "Loading"; // �ε� �� �̸�
    private static string targetSceneName; // �̵��� ��ǥ �� �̸� (���� ������ ����)
    //public string targetSceneName; // �̵��� ��ǥ �� �̸�

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹�ߴ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("bumped");

            // ��Ż �̸��� ���� �̵��� �� ����
            if (gameObject.name == "Obst_Run_Portal")
            {
                targetSceneName = "Obstacle_run"; // ù ��° ���� ��
            }
            else if (gameObject.name == "GR_Light_Portal")
            {
                targetSceneName = "Red_Green_light"; // �� ��° ���� ��
            }

            // �ε� ������ �̵�
            SceneManager.LoadScene(loadingSceneName);
        }
    }
    // ���� �޼���� ��ǥ �� �̸� ��������
    public static string GetTargetSceneName()
    {
        return targetSceneName; // ��ǥ �� �̸�
    }

}

