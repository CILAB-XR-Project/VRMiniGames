using System.Collections;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    public float forceStrength = 500f; // �浹 �� ������ ���� ����
    public float destroyDelay = 1f; // ��ֹ��� ���ŵ� ���� �ð�

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �÷��̾�� �浹���� ���� �����ϵ��� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false; // ���� ȿ�� ������ ���� kinematic ��Ȱ��ȭ

            // �浹 �� ���� ���մϴ�
            Vector3 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * forceStrength);

            // ���� �ð��� ���� �� ��ֹ��� �����մϴ�
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        // ������ �ð���ŭ ����� ��
        yield return new WaitForSeconds(destroyDelay);

        // ��ֹ� ����
        Destroy(gameObject);
    }
}

