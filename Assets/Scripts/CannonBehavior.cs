using UnityEngine;

public class CannonBehavior : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [Header("Punt de sortida de la bala")]
    [SerializeField] private Transform firePoint;

    [Header("Cadència de tir")]
    [SerializeField] private float fireRate = 1.5f;

    [Header("Detecció i Rotació")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("Ajusta aquest angle si el model 3D no mira cap endavant per defecte (ex: 90, -90, 180)")]
    [SerializeField] private float rotationOffset = 0f;

    private float timer;
    private Transform targetEnemy;

    private void Update()
    {
        // 1. Cerca l'esquelet més proper per apuntar-hi
        FindClosestEnemy();

        // 2. S'orienta cap a l'enemic si en troba un
        if (targetEnemy != null)
        {
            RotateTowardsTarget();
        }

        // 3. Dispar constant ininterromput
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void FindClosestEnemy()
    {
        SkeletonBehavior[] enemies = FindObjectsByType<SkeletonBehavior>(FindObjectsSortMode.None);
        SkeletonBehavior closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (SkeletonBehavior enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= detectionRange)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }

        targetEnemy = closest != null ? closest.transform : null;
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = targetEnemy.position - transform.position;
        direction.y = 0; // Mantenim la rotació plana en l'eix horitzontal

        if (direction.sqrMagnitude > 0.001f)
        {
            // Calculem la rotació cap a l'enemic i li sumem el desfasament (offset)
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, rotationOffset, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;

        Instantiate(bulletPrefab, spawnPos, spawnRot);
    }
}