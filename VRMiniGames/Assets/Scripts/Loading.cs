using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        // 5초 동안 로딩 화면 유지
        yield return new WaitForSeconds(5f);

        // Portal 스크립트의 정적 메서드를 사용해 목표 씬 이름 가져오기
        string targetSceneName = Portal.GetTargetSceneName();

        // 다음 씬 비동기로 로드
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        asyncOperation.allowSceneActivation = false; // 로딩 완료 전까지 씬 전환 멈춤

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true; // 로딩 완료 후 씬 활성화
            }
            yield return null;
        }
    }
}
