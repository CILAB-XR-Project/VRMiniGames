using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public FadeScreen fadeScreen; // FadeScreen
    public float waitTime = 5f;  // Waiting Time in Loading
    public AudioSource audioSource; // AudioSource for playing sound

    private void Start()
    {
        string targetSceneName = Portal.GetTargetSceneName();
        if (targetSceneName == "Red_Green_light")
        {
            if (audioSource != null)
            {
                audioSource.Play(); // Play the audio
            }
        }
        StartCoroutine(LoadTargetSceneWithFade(targetSceneName));
    }

    private IEnumerator LoadTargetSceneWithFade(string targetSceneName)
    {
        // Fade In
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Waiting
        yield return new WaitForSeconds(waitTime);

        // Fade Out
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Go to Target Scene
        SceneManager.LoadScene(targetSceneName);
    }
}