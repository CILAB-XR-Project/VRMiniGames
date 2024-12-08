using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    //private void OnTriggerEnter(Collider other)
    //{
    //    // 플레이어와 충돌했는지 확인
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("bumped");
    //        // 포탈 이름에 따라 씬 로드
    //        if (gameObject.name == "Obst_Run_Portal")
    //        {
    //            SceneManager.LoadScene("Obstacle_run"); // "ObstacleRunScene" 씬으로 이동
    //        }
    //        else if (gameObject.name == "GR_Light_Portal")
    //        {
    //            SceneManager.LoadScene("Red_Green_light"); // "LightPuzzleScene" 씬으로 이동
    //        }
    //    }
    //}
    public string loadingSceneName = "Loading"; // 로딩 씬 이름
    private static string targetSceneName; // 이동할 목표 씬 이름 (정적 변수로 설정)
    //public string targetSceneName; // 이동할 목표 씬 이름

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

