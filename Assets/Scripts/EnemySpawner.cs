using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private float spawnInterval = 3f;

    [SerializeField] private float spawnRadius = 2.0f;

    private CastleBehavior targetCastle;
    private float timer;

    private void Update()
    {
        // Search castle.
        if (targetCastle == null)
        {
            targetCastle = FindAnyObjectByType<CastleBehavior>();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnSkeletonAroundCastle();
            timer = 0f;
        }
    }

    private void SpawnSkeletonAroundCastle()
    {
        if (skeletonPrefab == null || targetCastle == null) return;

        // This code creates a radius arround the castle and instantiates skeletons in the border of this radius.

        Vector2 randomCircle = Random.insideUnitCircle.normalized; // We normalize the vector to be in the border of the radius.

        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * spawnRadius;

        Vector3 spawnPosition = targetCastle.transform.position + offset;

        Vector3 lookDirection = (targetCastle.transform.position - spawnPosition).normalized;
        lookDirection.y = 0;
        Quaternion spawnRotation = Quaternion.LookRotation(lookDirection);

        Instantiate(skeletonPrefab, spawnPosition, spawnRotation);
    }
}