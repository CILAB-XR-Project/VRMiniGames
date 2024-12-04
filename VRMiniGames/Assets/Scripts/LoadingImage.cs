using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingImage : MonoBehaviour
{
    public Image[] sceneImages; // Array of UI Images for each scene
    // Start is called before the first frame update
    void Start()
    {
        // Get the target scene name
        string targetSceneName = Portal.GetTargetSceneName();

        // Update UI image based on the target scene
        UpdateSceneImage(targetSceneName);
    }

    private void UpdateSceneImage(string sceneName)
    {
        // Hide all images first
        foreach (Image img in sceneImages)
        {
            img.gameObject.SetActive(false);
        }

        // Show the image for the specified scene
        switch (sceneName)
        {
            case "Obstacle_run":
                sceneImages[0].gameObject.SetActive(true); // Activate the first image
                break;

            case "Red_Green_light":
                sceneImages[1].gameObject.SetActive(true); // Activate the second image
                break;

            default:
                Debug.LogWarning($"No image assigned for scene: {sceneName}");
                break;
        }
    }
}
