using UnityEngine;

public class RotatingCollectible : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Start()
    {
        // �±� ����
        gameObject.tag = "item";
    }

    void Update()
    {
        // ȸ�� ȿ��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
