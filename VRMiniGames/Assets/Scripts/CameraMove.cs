using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Transform playerTransform;
    Vector3 Offset;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerTransform=GameObject.FindGameObjectWithTag("Player").transform;
        Offset = transform.position - playerTransform.position;
    }

    void LateUpdate()
    {
        transform.position = playerTransform.position + Offset;
    }
}
