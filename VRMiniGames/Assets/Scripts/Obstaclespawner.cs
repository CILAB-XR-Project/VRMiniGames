using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private float[] lanePositionsX = { -4f, 0f, 4f }; // 세 개의 라인 위치 (X축 좌표)

    public GameObject[] obstaclePrefabs; // 여러 장애물 프리팹을 담을 배열
    public GameObject collectiblePrefab; // 먹을 수 있는 프리팹
    public int numberOfObstacles = 6;
    public float spawnRangeZ = 4f;
    public float forceStrength = 500f; // 충돌 시 가해질 힘의 세기
    public float destroyDelay = 1f; // 장애물이 제거될 지연 시간
    public float minDistanceBetweenObstacles = 2f; // 장애물 간 최소 거리

    private List<float> usedZPositions = new List<float>(); // 이미 사용된 Z축 위치 저장

    void Start()
    {
        // Roads 오브젝트의 모든 자식을 순회
        foreach (Transform road in transform)
        {
            SpawnObstaclesOnRoad(road);
        }
    }

    void SpawnObstaclesOnRoad(Transform roadBlock)
    {
        usedZPositions.Clear(); // 새로운 Road마다 Z축 위치 초기화

        // 무작위로 제외할 라인 선택
        int excludedLaneIndex = Random.Range(0, lanePositionsX.Length);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            // 랜덤하게 장애물 프리팹 선택
            GameObject selectedObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // 무작위로 제외한 라인을 피해서 X축 위치 선택
            float laneX;
            do
            {
                laneX = lanePositionsX[Random.Range(0, lanePositionsX.Length)];
            } while (laneX == lanePositionsX[excludedLaneIndex]);

            // Z축 위치 생성 (겹치지 않도록)
            float spawnZ;
            int attempts = 0; // 무한 루프 방지용
            do
            {
                spawnZ = Random.Range(-spawnRangeZ, spawnRangeZ);
                attempts++;
            } while (IsZPositionUsed(spawnZ) && attempts < 100); // 겹치지 않도록 검사

            if (attempts >= 100)
            {
                Debug.LogWarning("SpawnObstacles: Z-position attempts exceeded. Check spawn range or distance settings.");
                continue;
            }

            // 사용된 Z축 위치로 저장
            usedZPositions.Add(spawnZ);

            // 장애물 위치 설정
            Vector3 spawnPosition = roadBlock.position + new Vector3(laneX, 0, spawnZ);

            // 장애물 생성
            if (Random.value > 0.7f)
            {
                GameObject spawnedObstacle = Instantiate(selectedObstacle, spawnPosition, Quaternion.identity);
                spawnedObstacle.tag = "obstacle"; // 태그 설정
                spawnedObstacle.transform.localScale *= 2f; // 크기를 두 배로 설정

                // Rigidbody 컴포넌트를 추가하고 초기 설정
                Rigidbody rb = spawnedObstacle.AddComponent<Rigidbody>();
                rb.isKinematic = true; // 기본적으로 비활성화

                // ObstacleBehavior 스크립트를 추가하여 충돌 시 날아가는 효과와 제거를 관리
                ObstacleBehavior behavior = spawnedObstacle.AddComponent<ObstacleBehavior>();
                behavior.forceStrength = forceStrength;
                behavior.destroyDelay = destroyDelay;

                // SphereCollider 추가
                SphereCollider obstacleCollider = spawnedObstacle.AddComponent<SphereCollider>();
                obstacleCollider.isTrigger = false; // 기본적으로 충돌 처리
            }
        }

        // 제외된 라인에 먹을 수 있는 프리팹 배치
        float collectibleSpawnZ = Random.Range(-spawnRangeZ, spawnRangeZ); // Z축 위치 랜덤 설정
        Vector3 collectiblePosition = roadBlock.position + new Vector3(lanePositionsX[excludedLaneIndex], 2, collectibleSpawnZ);
        GameObject spawnedCollectible = Instantiate(collectiblePrefab, collectiblePosition, Quaternion.identity);
        spawnedCollectible.tag = "item"; // 태그 설정

        // SphereCollider 추가
        SphereCollider collectibleCollider = spawnedCollectible.AddComponent<SphereCollider>();
        collectibleCollider.isTrigger = true; // 아이템은 트리거로 설정

        // 회전 스크립트 추가
        spawnedCollectible.AddComponent<RotatingCollectible>();
    }

    // Z축 위치가 이미 사용되었는지 확인
    bool IsZPositionUsed(float zPosition)
    {
        foreach (float usedZ in usedZPositions)
        {
            if (Mathf.Abs(usedZ - zPosition) < minDistanceBetweenObstacles)
            {
                return true; // 겹친다면 사용 불가
            }
        }
        return false;
    }
}
