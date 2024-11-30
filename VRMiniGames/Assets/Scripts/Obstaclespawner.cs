using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private float[] lanePositionsX = { -4f, 0f, 4f }; // �� ���� ���� ��ġ (X�� ��ǥ)

    public GameObject[] obstaclePrefabs; // ���� ��ֹ� �������� ���� �迭
    public GameObject collectiblePrefab; // ���� �� �ִ� ������
    public int numberOfObstacles = 6;
    public float spawnRangeZ = 4f;
    public float forceStrength = 500f; // �浹 �� ������ ���� ����
    public float destroyDelay = 1f; // ��ֹ��� ���ŵ� ���� �ð�
    public float minDistanceBetweenObstacles = 2f; // ��ֹ� �� �ּ� �Ÿ�

    private List<float> usedZPositions = new List<float>(); // �̹� ���� Z�� ��ġ ����

    void Start()
    {
        // Roads ������Ʈ�� ��� �ڽ��� ��ȸ
        foreach (Transform road in transform)
        {
            SpawnObstaclesOnRoad(road);
        }
    }

    void SpawnObstaclesOnRoad(Transform roadBlock)
    {
        usedZPositions.Clear(); // ���ο� Road���� Z�� ��ġ �ʱ�ȭ

        // �������� ������ ���� ����
        int excludedLaneIndex = Random.Range(0, lanePositionsX.Length);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            // �����ϰ� ��ֹ� ������ ����
            GameObject selectedObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // �������� ������ ������ ���ؼ� X�� ��ġ ����
            float laneX;
            do
            {
                laneX = lanePositionsX[Random.Range(0, lanePositionsX.Length)];
            } while (laneX == lanePositionsX[excludedLaneIndex]);

            // Z�� ��ġ ���� (��ġ�� �ʵ���)
            float spawnZ;
            int attempts = 0; // ���� ���� ������
            do
            {
                spawnZ = Random.Range(-spawnRangeZ, spawnRangeZ);
                attempts++;
            } while (IsZPositionUsed(spawnZ) && attempts < 100); // ��ġ�� �ʵ��� �˻�

            if (attempts >= 100)
            {
                Debug.LogWarning("SpawnObstacles: Z-position attempts exceeded. Check spawn range or distance settings.");
                continue;
            }

            // ���� Z�� ��ġ�� ����
            usedZPositions.Add(spawnZ);

            // ��ֹ� ��ġ ����
            Vector3 spawnPosition = roadBlock.position + new Vector3(laneX, 0, spawnZ);

            // ��ֹ� ����
            if (Random.value > 0.7f)
            {
                GameObject spawnedObstacle = Instantiate(selectedObstacle, spawnPosition, Quaternion.identity);
                spawnedObstacle.tag = "obstacle"; // �±� ����
                spawnedObstacle.transform.localScale *= 2f; // ũ�⸦ �� ��� ����

                // Rigidbody ������Ʈ�� �߰��ϰ� �ʱ� ����
                Rigidbody rb = spawnedObstacle.AddComponent<Rigidbody>();
                rb.isKinematic = true; // �⺻������ ��Ȱ��ȭ

                // ObstacleBehavior ��ũ��Ʈ�� �߰��Ͽ� �浹 �� ���ư��� ȿ���� ���Ÿ� ����
                ObstacleBehavior behavior = spawnedObstacle.AddComponent<ObstacleBehavior>();
                behavior.forceStrength = forceStrength;
                behavior.destroyDelay = destroyDelay;

                // SphereCollider �߰�
                SphereCollider obstacleCollider = spawnedObstacle.AddComponent<SphereCollider>();
                obstacleCollider.isTrigger = false; // �⺻������ �浹 ó��
            }
        }

        // ���ܵ� ���ο� ���� �� �ִ� ������ ��ġ
        float collectibleSpawnZ = Random.Range(-spawnRangeZ, spawnRangeZ); // Z�� ��ġ ���� ����
        Vector3 collectiblePosition = roadBlock.position + new Vector3(lanePositionsX[excludedLaneIndex], 2, collectibleSpawnZ);
        GameObject spawnedCollectible = Instantiate(collectiblePrefab, collectiblePosition, Quaternion.identity);
        spawnedCollectible.tag = "item"; // �±� ����

        // SphereCollider �߰�
        SphereCollider collectibleCollider = spawnedCollectible.AddComponent<SphereCollider>();
        collectibleCollider.isTrigger = true; // �������� Ʈ���ŷ� ����

        // ȸ�� ��ũ��Ʈ �߰�
    }

    // Z�� ��ġ�� �̹� ���Ǿ����� Ȯ��
    bool IsZPositionUsed(float zPosition)
    {
        foreach (float usedZ in usedZPositions)
        {
            if (Mathf.Abs(usedZ - zPosition) < minDistanceBetweenObstacles)
            {
                return true; // ��ģ�ٸ� ��� �Ұ�
            }
        }
        return false;
    }
}
