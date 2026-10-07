using System.Collections.Generic;
using UnityEngine;


public class CannonBehavior : MonoBehaviour
{
    [Header("Prefabs i Referències")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("Objecte fill que indica exactament el punt de sortida de la bala")]
    [SerializeField] private Transform firePoint;

    [Header("Cadència de tir")]
    [SerializeField] private float fireRate = 1.5f;

    [Header("Velocitat de rotacio")]
    [SerializeField] private float rotSpeed = 1.0f;

    [Header("Obstacles")]
    [SerializeField] private LayerMask obstacleLayer;

    public List<Transform> enemies = new List<Transform>();
    private Quaternion currentRotation;
    private float timer;

    private void Start()
    {
        currentRotation = transform.rotation;
    }

    private void Update()
    {
        enemies.RemoveAll(enemy => enemy == null);
        transform.rotation = currentRotation;

        if (enemies.Count == 0) return;

        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Transform t in enemies)
        {
            if (t == null) continue;

            if (HasObstacleInBetween(t.position))
            {
                continue;
            }

            float currentDist = Vector3.Distance(transform.position, t.position);
            if (currentDist < nearestDistance)
            {
                nearestDistance = currentDist;
                nearestEnemy = t;
            }
        }

        if (nearestEnemy != null)
        {
            Vector3 direction = nearestEnemy.position - transform.position;

            if (direction != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
                newRotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotSpeed*Time.deltaTime);
                transform.rotation = newRotation;
                currentRotation = newRotation;
            }
        }

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Instantiate(bulletPrefab, firePoint.position, transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firePoint.position, 0.03f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemies.Add(other.gameObject.transform);
        }
            
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemies.Remove(other.gameObject.transform);
        }
            
    }

    private bool HasObstacleInBetween(Vector3 targetPosition)
    {
        Vector3 startPosition = firePoint != null ? firePoint.position : transform.position;
        Vector3 direction = targetPosition - startPosition;
        float distance = direction.magnitude;

        return Physics.Raycast(startPosition, direction.normalized, distance, obstacleLayer);
    }
}