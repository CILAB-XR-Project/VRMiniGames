using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float speed = 1.0f;

    private Rigidbody myRigid;


    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    // 이벤트 리스트에 등록 및 해제


    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    void Move()
    {
        //float _move_dir_X = Input.GetAxisRaw("Horizontal");
        //float _move_dir_Z = Input.GetAxisRaw("Vertical");

        //Vector3 _move_horizontal = transform.right * _move_dir_X;
        //Vector3 _move_vertical = transform.forward * _move_dir_Z;

        //Vector3 _velocity = (_move_horizontal + _move_vertical).normalized * speed;

        //myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

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

}
