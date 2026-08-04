using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float horizontalLimit = 9f;
    [SerializeField] private float verticalLimit = 4f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner has no enemy prefabs assigned.");
            return;
        }

        Vector2 spawnPosition = GetRandomEdgePosition();

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private Vector2 GetRandomEdgePosition()
    {
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0:
                return new Vector2(
                    Random.Range(-horizontalLimit, horizontalLimit),
                    verticalLimit
                );

            case 1:
                return new Vector2(
                    Random.Range(-horizontalLimit, horizontalLimit),
                    -verticalLimit
                );

            case 2:
                return new Vector2(
                    -horizontalLimit,
                    Random.Range(-verticalLimit, verticalLimit)
                );

            default:
                return new Vector2(
                    horizontalLimit,
                    Random.Range(-verticalLimit, verticalLimit)
                );
        }
    }
}