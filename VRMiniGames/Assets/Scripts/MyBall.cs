using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyBall : MonoBehaviour
{
    public float Jump_power;
    bool isJump;
    public int itemcount;
    Rigidbody rigid;
    private float forceStrength = 500f;
    public int maxitemcount = 5;
    public float destroyDelay = 2f;
    public float finishDelay = 10f;

    public AudioClip itemSound; // �����ۿ� �ε����� �� ����� ����� Ŭ��
    public AudioClip obstacleSound; // ��ֹ��� �ε����� �� ����� ����� Ŭ��
    private AudioSource audioSource;
    

    private InputManager inputManager;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        inputManager = FindObjectOfType<InputManager>();
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "item")
        {
            PlaySound(itemSound);
            other.gameObject.SetActive(false);

            if (itemcount < maxitemcount)
            {
                itemcount++;
                print(itemcount);
            }
            else
            {
                print("Fever has been activated.");
            }
        }
        else if (other.tag == "obstacle")
        {
            PlaySound(obstacleSound);
            Rigidbody otherRigidbody = other.attachedRigidbody;
            inputManager.speed = inputManager.speed / 2;

            if (otherRigidbody != null)
            {
                otherRigidbody.isKinematic = false;

                Vector3 forceDirection = (other.transform.position - transform.position).normalized;
                otherRigidbody.AddForce(forceDirection * forceStrength);
            }

            StartCoroutine(DestroyAfterDelay(other.gameObject));
        }
        else if (other.tag == "Finish")
        {
            StartCoroutine(WaitAndExecute());
        }
        else if (other.tag == "leftwall")
        {
            inputManager.leftwall = true; 
        }
        else if (other.tag == "rightwall")
        {
            inputManager.rightwall = true; 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "leftwall")
        {
            inputManager.leftwall = false; 
        }
        else if (other.tag == "rightwall")
        {
            inputManager.rightwall = false; 
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obstacle)
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(obstacle);
    }

    private IEnumerator WaitAndExecute()
    {

        
        yield return new WaitForSeconds(finishDelay);

        //Game Clear!
        SceneManager.LoadScene("Lobby");
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // ������ ����� Ŭ�� ���
        }
    }
}