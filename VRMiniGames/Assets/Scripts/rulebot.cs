using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace rulebot
{
    public class rulebot : MonoBehaviour
    {
        private AnimationClip[] myClips;
        private Animator _animator;
        private Animator tagger_animator;
        private Rigidbody _rb;
        
        private string currentMovementState = "idle";
        [SerializeField]
        private float walkingSpeed = 1.0f;
        [SerializeField]
        private float joggingSpeed = 2.0f;
        [SerializeField]
        private float runningSpeed = 3.0f;
        [SerializeField]
        private float squartProb = 0.95f;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            tagger_animator = GameObject.Find("Younghee").GetComponent<Animator>();
            if (_animator != null)
            {
                myClips = _animator.runtimeAnimatorController.animationClips;
                StartCoroutine(CheckTaggerState());
            }
        }
        
        void Update()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (currentMovementState == "walking" && stateInfo.IsName("walking"))
            {
                MoveForward(walkingSpeed);
            }
            else if (currentMovementState == "jogging" && stateInfo.IsName("jogging"))
            {
                MoveForward(joggingSpeed);
            }
            else if (currentMovementState == "running" && stateInfo.IsName("running"))
            {
                MoveForward(runningSpeed);
            }
        }
        
        private void MoveForward(float speed)
        {
            Vector3 moveDirection = transform.forward * speed * Time.deltaTime;
            // transform.position += moveDirection;
            _rb.MovePosition(transform.position + moveDirection);
        }

        IEnumerator CheckTaggerState()
        {
            while (true)
            {
                var stateInfo = tagger_animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("idle") || stateInfo.IsName("singing") || stateInfo.IsName("turning"))
                {
                    string[] states = { "walking", "jogging", "running" };
                    currentMovementState = states[Random.Range(0, states.Length)];
                    SetAnimatorState(currentMovementState);
                }
                else if (stateInfo.IsName("checking"))
                {
                    if (Random.value < squartProb)
                    {
                        currentMovementState = "squart";
                        SetAnimatorState("squart");
                    }
                    else
                    {
                        string[] states = { "walking", "jogging", "running" };
                        currentMovementState = states[Random.Range(0, states.Length)];
                        SetAnimatorState(currentMovementState);
                    }
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void SetAnimatorState(string state)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isJogging", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isSquart", false);
        
            switch (state)
            {
                case "walking":
                    _animator.SetBool("isWalking", true);
                    break;
                case "jogging":
                    _animator.SetBool("isJogging", true);
                    break;
                case "running":
                    _animator.SetBool("isRunning", true);
                    break;
                case "squart":
                    _animator.SetBool("isSquart", true);
                    break;
                default:
                    Debug.LogWarning("Unknown state requested");
                    break;
            }
        }

    }
}