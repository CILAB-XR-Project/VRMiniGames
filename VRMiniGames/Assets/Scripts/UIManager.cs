using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private string cur_action;
    private string prev_action;
    public Animator ui_animator;

    private string currentSceneName = "";

    //game timer vars
    private bool is_game_timer_UI = false;
    private float start_time;
    private float elapsed_time;
    public TextMeshProUGUI gamer_timer_text;

    //fever vars
    private bool is_fever_UI = false;
    private bool is_fever_able = false;
    private int cur_fever_item_cnt = 0;
    private int max_fever_item_cnt = 5;
    private float[] fever_range = new float[2] { 0, 100 };
    public TextMeshProUGUI fever_item_text;
    public Image fever_ready_icon;


    private void Start()
    {
        cur_action = "";
        prev_action = "";
        currentSceneName = SceneManager.GetActiveScene().name;

        elapsed_time = 0.0f;
        start_time = 0.0f;
        fever_item_text.gameObject.SetActive(false);
        gamer_timer_text.gameObject.SetActive(false);
        fever_item_text.gameObject.SetActive(false);
        fever_ready_icon.gameObject.SetActive(false);

    }

    void UpdatePythonAction(ModelOutputData data)
    {
        prev_action = cur_action;
        cur_action = data.GetAction();

        if (currentSceneName == "Obstacle_run" && cur_action == "squat")
        {
            cur_action = "others";
        }
        else if (currentSceneName == "Red_Green_light" && cur_action == "lunge")
        {
            cur_action = "others";
        }


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
        UpdateActionAnimationUI();
        UpdateGameTimerUI();
        UpdateFeverUI();
    }

    //action status ui function
    private void UpdateActionAnimationUI()
    {
        if (cur_action != prev_action)
            ui_animator.SetBool($"is_{prev_action}", false);
        ui_animator.SetBool($"is_{cur_action}", true);
    }


    //game timer ui functions
    public void StartGameTimerUI() 
    {
        is_game_timer_UI = true;
        start_time = Time.timeSinceLevelLoad;
        gamer_timer_text.gameObject.SetActive(true);

    }

    private void UpdateGameTimerUI()
    {
        if (is_game_timer_UI)
        {
            float elapsed = Time.timeSinceLevelLoad - start_time;

            int minutes = (int)(elapsed / 60);
            float seconds = elapsed % 60f;

            gamer_timer_text.text = string.Format("{0:00}:{1:00.0}", minutes, seconds);
        }
    }

    public void EndGameTimer()
    {
        is_game_timer_UI = false;
    }

    public float GetGameTime() { return elapsed_time; }

    //fever ui function
    public void StartFeverUI()
    {
        is_fever_UI = true;
        fever_item_text.gameObject.SetActive(true);

    }


    private void UpdateFeverUI()
    {
        if (is_fever_UI)
        {
            fever_item_text.text = $"Items: {cur_fever_item_cnt} / {max_fever_item_cnt}";
            if (cur_fever_item_cnt == max_fever_item_cnt)
            {
                fever_ready_icon.gameObject.SetActive(true);
            }
            else
            {
                fever_ready_icon.gameObject.SetActive(false);
            }
        }
    }

    public void TriggerFever()
    {
        fever_ready_icon.gameObject.SetActive(false);
        cur_fever_item_cnt = 0;

    }

    public void SetFeverItemCnt(int item_cnt)
    {
        cur_fever_item_cnt = item_cnt;
    }

    public void EndFeverUI()
    {
        is_fever_UI = false;
        fever_ready_icon.gameObject.SetActive(false);
        fever_item_text.gameObject.SetActive(false);
    }

}
