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

        // move character by keyboard(for debug)
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 hmd_left = -hmd_transform.right;
            hmd_left.y = 0f;
            transform.position += hmd_left * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 hmd_right = hmd_transform.right;
            hmd_right.y = 0f;
            transform.position += hmd_right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 hmd_front = hmd_transform.forward;
            hmd_front.y = 0f;
            transform.position += hmd_front * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 hmd_backward = -hmd_transform.forward;
            hmd_backward.y = 0f;
            transform.position += hmd_backward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E)) 
        {
            transform.Rotate(Vector3.up, 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q)) 
        {
            transform.Rotate(Vector3.up, -100 * Time.deltaTime);
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
