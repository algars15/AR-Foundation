using UnityEngine;

public class CannonLookAt : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private void Update()
    {
        SkeletonBehavior[] enemies = FindObjectsByType<SkeletonBehavior>(FindObjectsSortMode.None);
        SkeletonBehavior closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (SkeletonBehavior enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null && shortestDistance <= range)
        {
            Vector3 direction = closestEnemy.transform.position - transform.position;
            direction.y = 0; // Manté el canó horitzontal

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}