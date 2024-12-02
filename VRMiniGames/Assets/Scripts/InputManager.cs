using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InputManager : MonoBehaviour
{
    //Moving speed
    public float speed = 1.0f;

    //Currnet action from tactile
    private string cur_action;

    // Variables for moving direction
    public Transform hmd_transform;
    private Vector3 front_direction = Vector3.forward;
    private bool move_only_front = false;


    public TMP_Text action_text;
    //public Transform character;

    //private void Start()
    //{
    //    Vector3 character_pos = character.position;
    //    character_pos.y = 1.3f;
    //    transform.position = character_pos;
    //}

    //Socket communication evnent listener functions
    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += UpdatePythonAction;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= UpdatePythonAction;
    }
    void UpdatePythonAction(ModelOutputData data)
    {
        this.cur_action = data.GetAction();
        print($"Action: {cur_action}");
    }

    void Update()
    {
        Move();
        action_text.text = cur_action;
    }


    void Move()
    {
        if (cur_action == "walk")
        {   
            if (move_only_front)
            {
                transform.position = front_direction * speed * Time.deltaTime;
            }
            else
            {
                Vector3 hmd_front = hmd_transform.forward;
                hmd_front.y = 0f;
                transform.position += hmd_front * speed * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }
    }

    //Setters
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetFront(Vector3 front)
    {
        this.front_direction = front;
    }

    public void SetOnlyMovingFront(bool move_front)
    {
        this.move_only_front = move_front;
    }

    //

}
