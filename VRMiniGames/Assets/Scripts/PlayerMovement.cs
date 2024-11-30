using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도 조정

    void Update()
    {
        // 방향키 입력 받기
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // 이동
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
