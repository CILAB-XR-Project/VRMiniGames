using System.Collections;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    public float forceStrength = 500f; // 충돌 시 가해질 힘의 세기
    public float destroyDelay = 1f; // 장애물이 제거될 지연 시간

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했을 때만 반응하도록 설정
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false; // 물리 효과 적용을 위해 kinematic 비활성화

            // 충돌 시 힘을 가합니다
            Vector3 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * forceStrength);

            // 일정 시간이 지난 후 장애물을 제거합니다
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        // 지정된 시간만큼 대기한 후
        yield return new WaitForSeconds(destroyDelay);

        // 장애물 제거
        Destroy(gameObject);
    }
}

