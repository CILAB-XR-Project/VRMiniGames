using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MyBall : MonoBehaviour
{
    
    public float Jump_power;
    bool isJump;
    public int itemcount;
    public float velocity = 10;
    Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        // rigid.velocity = Vector3.left;
        // rigid.velocity = new Vector3(2, 4, -1);
        // rigid.AddForce(Vector3.up * 500, ForceMode.Impulse);

        
    }
    void Awake()
    {
        isJump = false;
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            isJump = true;
            rigid.AddForce(new Vector3(0,Jump_power,0), ForceMode.Impulse);
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        // rigid.velocity = new Vector3(2, 4, -1);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);

        // #3. ȸ����
       //rigid.AddTorque(Vector3.left);
    }
    void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "item")
        {
            other.gameObject.SetActive(false);
            if (itemcount < 5) {
                itemcount++;
                print(itemcount);
            }
            else
            {
                print("Fever �������� Ȱ��ȭ�Ǿ����ϴ�.");
            }
        }
        else if (other.tag == "obstacle")
        {
            velocity = velocity / 2;
            print(velocity);
        }
        else if (other.tag == "Finish")
        {
            print("�ð�");

            //Game Clear!
            SceneManager.LoadScene("Lobby");
        }
    }

}