using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToSprite : MonoBehaviour
{
    public Camera captureCamera; // 캡처용 카메라
    public int frameRate = 24;   // 초당 프레임
    public int totalFrames = 60; // 총 캡처 프레임 수
    public RenderTexture renderTexture;
    public string outputPath = "Assets/AnimationFrame/";

    private int currentFrame = 0;

    void Start()
    {
        InvokeRepeating("CaptureFrame", 0f, 1f / frameRate);
    }

    void CaptureFrame()
    {
        if (currentFrame >= totalFrames)
        {
            CancelInvoke("CaptureFrame");
            Debug.Log("All frames captured.");
            return;
        }

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{outputPath}frame_{currentFrame:D4}.png", bytes);

        RenderTexture.active = null;
        currentFrame++;
    }
}
