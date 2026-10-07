using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private float spawnInterval = 3f;

    [SerializeField] private float spawnRadius = 2.0f;

    public void SpawnSkeletonAroundCastle()
    {
        if (skeletonPrefab == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;

        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * spawnRadius;

        Vector3 spawnPosition = transform.position + offset;

        Vector3 lookDirection = (transform.position - spawnPosition).normalized;
        lookDirection.y = 0;
        Quaternion spawnRotation = Quaternion.LookRotation(lookDirection);

        Instantiate(skeletonPrefab, spawnPosition, spawnRotation);
    }
}