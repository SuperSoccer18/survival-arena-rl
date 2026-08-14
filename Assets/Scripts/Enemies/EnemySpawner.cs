using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;
    [SerializeField] private GameObject shooterEnemyPrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;
    [SerializeField] private float spawnAcceleration = 0.015f;

    [Header("Arena Limits")]
    [SerializeField] private float horizontalLimit = 9f;
    [SerializeField] private float verticalLimit = 4f;
    [SerializeField] private bool testShooter = false;

    private float elapsedTime;
    private float spawnTimer;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        float currentInterval = Mathf.Max(
            minimumSpawnInterval,
            initialSpawnInterval - elapsedTime * spawnAcceleration
        );

        if (spawnTimer >= currentInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        GameObject selectedPrefab = SelectEnemyPrefab();
        Vector2 spawnPosition = GetRandomEdgePosition();

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private GameObject SelectEnemyPrefab()
    {
        // If testing the shooter enemy, always spawn it.
        if (testShooter)
        {
            return shooterEnemyPrefab;
        }

        float roll = Random.value;

        // First 20 seconds: only normal enemies.
        if (elapsedTime < 20f)
        {
            return normalEnemyPrefab;
        }

        // 20–40 seconds: normal and fast.
        if (elapsedTime < 40f)
        {
            return roll < 0.7f
                ? normalEnemyPrefab
                : fastEnemyPrefab;
        }

        // 40–60 seconds: introduce tanks.
        if (elapsedTime < 60f)
        {
            if (roll < 0.55f)
            {
                return normalEnemyPrefab;
            }

            if (roll < 0.8f)
            {
                return fastEnemyPrefab;
            }

            return tankEnemyPrefab;
        }

        // After 60 seconds: all four types.
        if (roll < 0.4f)
        {
            return normalEnemyPrefab;
        }

        if (roll < 0.65f)
        {
            return fastEnemyPrefab;
        }

        if (roll < 0.82f)
        {
            return tankEnemyPrefab;
        }

        return shooterEnemyPrefab;
    }

    private Vector2 GetRandomEdgePosition()
    {
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(
                Random.Range(-horizontalLimit, horizontalLimit),
                verticalLimit
            ),

            1 => new Vector2(
                Random.Range(-horizontalLimit, horizontalLimit),
                -verticalLimit
            ),

            2 => new Vector2(
                -horizontalLimit,
                Random.Range(-verticalLimit, verticalLimit)
            ),

            _ => new Vector2(
                horizontalLimit,
                Random.Range(-verticalLimit, verticalLimit)
            )
        };
    }
}