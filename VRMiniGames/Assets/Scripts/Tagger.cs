using System.Collections;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private Transform _head;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float rotationSpeed = 90f;
    public float pauseAfterTurn = 1f;
    
    private void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogError("There should be at least two AudioSources in the object.");
            return;
        }

        _audioSource1 = audioSources[0];
        _audioSource2 = audioSources[1];

        _head = transform.Find("DollHead");
        
        StartCoroutine(PlayAudioAndRotate());
    }
    
    // PlayAudioAndRotate Coroutine
    private IEnumerator PlayAudioAndRotate()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            
            if (!_audioSource1.isPlaying)
            {
                _audioSource1.Play();
            }
            
            while (_audioSource1.isPlaying)
            {
                yield return null;
            }
            
            if (!_audioSource2.isPlaying)
            {
                _audioSource2.Play();
            }
            yield return StartCoroutine(SmoothRotate(180f));
            
            yield return new WaitForSeconds(pauseAfterTurn);
            
            yield return StartCoroutine(SmoothRotate(180f));
        }
    }
    
    private IEnumerator SmoothRotate(float targetAngle)
    {
        float rotatedAngle = 0f;
        float rotationDirection = targetAngle > 0 ? 1 : -1;

        while (Mathf.Abs(rotatedAngle) < Mathf.Abs(targetAngle))
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            if (Mathf.Abs(rotatedAngle) + rotationStep > Mathf.Abs(targetAngle))
            {
                rotationStep = Mathf.Abs(targetAngle) - Mathf.Abs(rotatedAngle); // 초과 회전 방지
            }
            
            _head.Rotate(0, rotationStep * rotationDirection, 0);
            
            rotatedAngle += rotationStep;
            
            yield return null;
        }
    }
}
