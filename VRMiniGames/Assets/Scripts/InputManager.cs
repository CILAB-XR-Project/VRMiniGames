using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float speed = 1.0f;

    private int cur_action;
    public Transform hmd_transform;
    public Transform character;



    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += UpdatePythonAction;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= UpdatePythonAction;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private void Start()
    {
        Vector3 character_pos = character.position;
        character_pos.y = 1.5f;
        transform.position = character_pos;
    }

    void Move()
    {
        if (cur_action == 4 || cur_action == 5)
        {
            Vector3 hmd_front = hmd_transform.forward;
            hmd_front.y = 0f;
            transform.position += hmd_front * speed * Time.deltaTime;
        }
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(-speed * Time.deltaTime, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(speed * Time.deltaTime, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    transform.Translate(0, 0, speed * Time.deltaTime);
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    transform.Translate(0, 0, -speed * Time.deltaTime);
        //}
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    void UpdatePythonAction(ModelOutputData data)
    { 
        cur_action = data.GetAction();
    }

}
