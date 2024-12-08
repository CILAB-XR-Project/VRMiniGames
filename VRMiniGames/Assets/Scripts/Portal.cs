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
        // Check if player collided
        if (other.CompareTag("Player"))
        {
            Debug.Log("bumped");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StopLobbyMusic();
            }
            // Set scene to move based on portal name
            if (gameObject.name == "Obst_Run_Portal")
            {
                targetSceneName = "Obstacle_run"; 
            }
            else if (gameObject.name == "GR_Light_Portal")
            {
                targetSceneName = "Red_Green_light"; 
            }

            // Move to Loading Scene
            SceneManager.LoadScene(loadingSceneName);
        }
    }
    // Get target scene name with static method
    public static string GetTargetSceneName()
    {
        return targetSceneName; // Target Scene Name
    }

}

