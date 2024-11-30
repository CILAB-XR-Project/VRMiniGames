using UnityEngine;

public class RotatingCollectible : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Start()
    {
        // 태그 설정
        gameObject.tag = "item";
    }

    void Update()
    {
        // 회전 효과
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
