using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public FadeScreen fadeScreen; // ���̵� ȿ���� ���� FadeScreen
    public float waitTime = 5f;  // �ε� ȭ�鿡�� ��� �ð�

    private void Start()
    {
        StartCoroutine(LoadTargetSceneWithFade());
    }

    private IEnumerator LoadTargetSceneWithFade()
    {
        // ���̵� �ƿ� (ȭ�� ��ο���)
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // ��� �ð�
        yield return new WaitForSeconds(waitTime);

        // ���̵� �� (ȭ�� �����)
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // ��ǥ ������ ��ȯ
        string targetSceneName = Portal.GetTargetSceneName();
        SceneManager.LoadScene(targetSceneName);
    }
}