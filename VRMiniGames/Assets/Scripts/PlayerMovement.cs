using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ� ����

    void Update()
    {
        // ����Ű �Է� �ޱ�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // �̵� ���� ����
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // �̵�
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
