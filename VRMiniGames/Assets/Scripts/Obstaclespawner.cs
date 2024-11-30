using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private float[] lanePositionsX = { -4f, 0f, 4f };

    public GameObject[] obstaclePrefabs;
    public GameObject collectiblePrefab;
    public int numberOfObstacles = 6;
    public float spawnRangeZ = 4f;
    public float forceStrength = 500f;
    public float destroyDelay = 1f;
    public float minDistanceBetweenObstacles = 2f;

    private List<float> usedZPositions = new List<float>();

    void Start()
    {
        foreach (Transform road in transform)
        {
            SpawnObstaclesOnRoad(road);
        }
    }

    void SpawnObstaclesOnRoad(Transform roadBlock)
    {
        usedZPositions.Clear();
        
        int excludedLaneIndex = Random.Range(0, lanePositionsX.Length);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            GameObject selectedObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            
            float laneX;
            do
            {
                laneX = lanePositionsX[Random.Range(0, lanePositionsX.Length)];
            } while (laneX == lanePositionsX[excludedLaneIndex]);
            
            float spawnZ;
            int attempts = 0;
            do
            {
                spawnZ = Random.Range(-spawnRangeZ, spawnRangeZ);
                attempts++;
            } while (IsZPositionUsed(spawnZ) && attempts < 100);

            if (attempts >= 100)
            {
                Debug.LogWarning("SpawnObstacles: Z-position attempts exceeded. Check spawn range or distance settings.");
                continue;
            }
            
            usedZPositions.Add(spawnZ);
            
            Vector3 spawnPosition = roadBlock.position + new Vector3(laneX, 0, spawnZ);
            
            if (Random.value > 0.7f)
            {
                GameObject spawnedObstacle = Instantiate(selectedObstacle, spawnPosition, Quaternion.identity);
                spawnedObstacle.tag = "obstacle";
                spawnedObstacle.transform.localScale *= 2f;
                
                Rigidbody rb = spawnedObstacle.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                
                ObstacleBehavior behavior = spawnedObstacle.AddComponent<ObstacleBehavior>();
                behavior.forceStrength = forceStrength;
                behavior.destroyDelay = destroyDelay;
                
                SphereCollider obstacleCollider = spawnedObstacle.AddComponent<SphereCollider>();
                obstacleCollider.isTrigger = false;
            }
        }
        
        float collectibleSpawnZ = Random.Range(-spawnRangeZ, spawnRangeZ);
        Vector3 collectiblePosition = roadBlock.position + new Vector3(lanePositionsX[excludedLaneIndex], 2, collectibleSpawnZ);
        GameObject spawnedCollectible = Instantiate(collectiblePrefab, collectiblePosition, Quaternion.identity);
        spawnedCollectible.tag = "item";
        
        SphereCollider collectibleCollider = spawnedCollectible.AddComponent<SphereCollider>();
        collectibleCollider.isTrigger = true;
        
        // spawnedCollectible.AddComponent<RotatingCollectible>();
    }
    
    bool IsZPositionUsed(float zPosition)
    {
        foreach (float usedZ in usedZPositions)
        {
            if (Mathf.Abs(usedZ - zPosition) < minDistanceBetweenObstacles)
            {
                return true;
            }
        }
        return false;
    }
}
