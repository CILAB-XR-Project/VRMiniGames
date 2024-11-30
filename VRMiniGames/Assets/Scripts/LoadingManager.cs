using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public FadeScreen fadeScreen; // 페이드 효과를 위한 FadeScreen
    public float waitTime = 5f;  // 로딩 화면에서 대기 시간

    private void Start()
    {
        StartCoroutine(LoadTargetSceneWithFade());
    }

    private IEnumerator LoadTargetSceneWithFade()
    {
        // 페이드 아웃 (화면 어두워짐)
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // 대기 시간
        yield return new WaitForSeconds(waitTime);

        // 페이드 인 (화면 밝아짐)
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // 목표 씬으로 전환
        string targetSceneName = Portal.GetTargetSceneName();
        SceneManager.LoadScene(targetSceneName);
    }
}