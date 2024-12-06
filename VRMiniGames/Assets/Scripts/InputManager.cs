using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    private MyBall myBall;

    private bool Fevertime = false;
    public float Fever_velocity = 8f;
    public float Feverdelaytime = 3f;

    public bool leftwall = false;
    public bool rightwall = false;

    public float deceleration = 1;
    public float acceleration = 2;

    public float maxForwardSpeed = 4;
    private string currentSceneName = "";

    public TMP_Text action_text;
    //public Transform character;

    private void Start()
    {
        myBall = FindObjectOfType<MyBall>();
       currentSceneName = SceneManager.GetActiveScene().name;

        //    Vector3 character_pos = character.position;
        //    character_pos.y = 1.3f;
        //    transform.position = character_pos;
    }

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
        if (currentSceneName == "Obstacle_run") Obstacle_Move();
        else Move();
        // action_text.text = cur_action;
    }

    void Obstacle_Move()
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
        if (Input.GetKey(KeyCode.P) && myBall.itemcount == myBall.maxitemcount)
        {
            myBall.itemcount = 0;
            StartCoroutine(FevertDelay());
        }

        if (Input.GetKey(KeyCode.A) && !leftwall)
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D) && !rightwall)
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (!Fevertime)
                if (speed > maxForwardSpeed) speed -= deceleration * Time.deltaTime;
                else speed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (speed < -maxForwardSpeed) speed += deceleration * Time.deltaTime;
            else speed -= acceleration * Time.deltaTime;
        }
        else
        {
            if (speed > 0)
            {
                speed -= deceleration * Time.deltaTime;
                if (speed < 0) speed = 0;
            }
            else if (speed < 0)
            {
                speed += deceleration * Time.deltaTime;
                if (speed > 0) speed = 0;
            }
        }
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        transform.position += forwardDirection * speed * Time.deltaTime;
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
    private IEnumerator FevertDelay()
    {
        Fevertime = true;

        speed = Fever_velocity;
        yield return new WaitForSeconds(Feverdelaytime);

        Fevertime = false;
    }
}
