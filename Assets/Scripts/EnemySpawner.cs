using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject swordSkeletonPrefab;

    //[SerializeField] private float spawnInterval = 3f;

    [SerializeField] private float spawnRadius = 2.0f;

    [SerializeField] private float swordSkeletonChance = 0.25f;

    public void SpawnSkeletonAroundCastle()
    {
        // 1. Comprovem si hi ha almenys un canó instanciat a l'escena
        CannonBehavior cannon = FindAnyObjectByType<CannonBehavior>();
        if (cannon == null)
        {
            // Si encara no s'ha col·locat cap canó, cancel·lem l'aparició
            return;
        }

        // 2. Si hi ha canó, procedim a generar l'enemic
        GameObject prefabToSpawn = ChooseEnemyPrefab();

        if (prefabToSpawn == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;

        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * spawnRadius;

        Vector3 spawnPosition = transform.position + offset;

        Vector3 lookDirection = (transform.position - spawnPosition).normalized;
        lookDirection.y = 0;
        Quaternion spawnRotation = Quaternion.LookRotation(lookDirection);

        Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
    }

    private GameObject ChooseEnemyPrefab()
    {
        if (swordSkeletonPrefab == null) return skeletonPrefab;

        if (Random.value < swordSkeletonChance) return swordSkeletonPrefab;

        return skeletonPrefab;
    }
}