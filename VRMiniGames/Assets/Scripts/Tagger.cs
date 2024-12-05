using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Tagger : MonoBehaviour
{
    private Animator _animator;
    private AnimationClip[] _myClips;
    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private AudioSource _audioSource3;
    private AudioSource _audioSource4;
    private Transform _head;
    private string _userAction = "walk";
    
    [SerializeField]
    private GameObject bloodEffect;
    
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float rotationSpeed = 90f;
    public float pauseAfterTurn = 1f;
    public float pushForce = 200f;
    
    [SerializeField]
    private List<Animator> rulebotAnimators;

    [SerializeField] 
    private Transform userTrans;
    
    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += setUserAction;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= setUserAction;
    }
    
    private void setUserAction(ModelOutputData outputData)
    {
        _userAction = outputData.GetAction();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _myClips = _animator.runtimeAnimatorController.animationClips;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 3)
        {
            Debug.LogError("There should be at least two AudioSources in the object.");
            return;
        }

        _audioSource1 = audioSources[0];
        _audioSource2 = audioSources[1];
        _audioSource3 = audioSources[2];
        _audioSource4 = audioSources[3];

        _head = transform.Find("DollHead");
        
        rulebotAnimators = new List<Animator>();
        foreach (var rulebot in GameObject.FindGameObjectsWithTag("Player"))
        {
            rulebotAnimators.Add(rulebot.GetComponent<Animator>());
        }
        
        StartCoroutine(PlayAudioAndRotate());
    }
    
    private IEnumerator PlayAudioAndRotate()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            
            if (!_audioSource1.isPlaying)
            {
                _audioSource1.Play();
                SetAnimatorState("singing");
            }
            
            while (_audioSource1.isPlaying)
            {
                yield return null;
            }
            
            if (!_audioSource2.isPlaying)
            {
                yield return StartCoroutine(SmoothRotate(180f));
                yield return StartCoroutine(CheckPlayerMove(pauseAfterTurn));
                yield return StartCoroutine(SmoothRotate(180f));
            }
            
            SetAnimatorState("idle");
        }
    }
    
    private IEnumerator SmoothRotate(float targetAngle)
    {
        _audioSource2.Play();
        SetAnimatorState("turning");
        
        float rotatedAngle = 0f;
        float rotationDirection = targetAngle > 0 ? 1 : -1;

        while (Mathf.Abs(rotatedAngle) < Mathf.Abs(targetAngle))
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            if (Mathf.Abs(rotatedAngle) + rotationStep > Mathf.Abs(targetAngle))
            {
                rotationStep = Mathf.Abs(targetAngle) - Mathf.Abs(rotatedAngle);
            }
            
            _head.Rotate(0, rotationStep * rotationDirection, 0);
            
            rotatedAngle += rotationStep;
            
            yield return null;
        }
    }
    
    private IEnumerator CheckPlayerMove(float pauseTime)
    {
        SetAnimatorState("checking");
        yield return new WaitForSeconds(1.0f);
        _audioSource3.Play();
    
        float timer = 0f;
        HashSet<Animator> processedPlayers = new HashSet<Animator>();
        Boolean processedUser = false;

        while (timer < pauseTime)
        {
            foreach (Animator animator in rulebotAnimators)
            {
                if (processedPlayers.Contains(animator))
                {
                    continue;
                }

                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("idle") || stateInfo.IsName("walking") || stateInfo.IsName("jogging") || stateInfo.IsName("running"))
                {
                    Transform playerTransform = animator.transform;
                    GameObject effect = Instantiate(bloodEffect, playerTransform.position, Quaternion.identity);
                    Destroy(effect, 5.0f);

                    Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 pushDirection = Vector3.forward;
                        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    }
                    
                    processedPlayers.Add(animator);
                    StartCoroutine(PlaySoundWithDelay());
                }
            }
            
            if ((_userAction == "walk" || _userAction == "others") && !processedUser)
            {
                GameObject effect = Instantiate(bloodEffect, userTrans.position, Quaternion.identity);
                Destroy(effect, 5.0f);
                    
                Rigidbody rb = userTrans.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pushDirection = Vector3.forward;
                    rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                }
                    
                processedUser = true;
                StartCoroutine(PlaySoundWithDelay());
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator PlaySoundWithDelay()
    {
        _audioSource4.PlayOneShot(_audioSource4.clip);
        yield return null;
    }
    
    private void SetAnimatorState(string state)
    {
        _animator.SetBool("isIdle", false);
        _animator.SetBool("isSinging", false);
        _animator.SetBool("isTurning", false);
        _animator.SetBool("isChecking", false);
        
        switch (state)
        {
            case "idle":
                _animator.SetBool("isIdle", true);
                break;
            case "singing":
                _animator.SetBool("isSinging", true);
                break;
            case "turning":
                _animator.SetBool("isTurning", true);
                break;
            case "checking":
                _animator.SetBool("isChecking", true);
                break;
            default:
                Debug.LogWarning("Unknown state requested");
                break;
        }
    }
}
