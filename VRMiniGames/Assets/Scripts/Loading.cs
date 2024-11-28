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
        // 5�� ���� �ε� ȭ�� ����
        yield return new WaitForSeconds(5f);

        // Portal ��ũ��Ʈ�� ���� �޼��带 ����� ��ǥ �� �̸� ��������
        string targetSceneName = Portal.GetTargetSceneName();

        // ���� �� �񵿱�� �ε�
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        asyncOperation.allowSceneActivation = false; // �ε� �Ϸ� ������ �� ��ȯ ����

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true; // �ε� �Ϸ� �� �� Ȱ��ȭ
            }
            yield return null;
        }
    }
}
