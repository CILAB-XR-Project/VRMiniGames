using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    private string cur_action;
    private string prev_action;
    public Animator ui_animator;

    private void Start()
    {
        cur_action = "";
        prev_action = "";
    }
    void UpdatePythonAction(ModelOutputData data)
    {
        prev_action = cur_action;
        cur_action = data.GetAction();

        print($"Action: {cur_action}");
        
    }
    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += UpdatePythonAction;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= UpdatePythonAction;
    }

    private void Update()
    {   
        if(cur_action!=prev_action)
            ui_animator.SetBool($"is_{prev_action}", false);
            ui_animator.SetBool($"is_{cur_action}", true);
    }
}
