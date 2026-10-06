using UnityEngine;

public class SkeletonBehavior : MonoBehaviour
{
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;

    private Transform targetCastle;

    private void Start()
    {
        // Search the castle in the scene.
        CastleBehavior castle = FindAnyObjectByType<CastleBehavior>();
        if (castle != null)
        {
            targetCastle = castle.transform;
        }
    }

    private void Update()
    {
        if (targetCastle == null) return;

        // Instanciate the direction towards the castle and lock Y Axis.
        Vector3 direction = (targetCastle.position - transform.position).normalized;
        direction.y = 0; // Mantenir-lo horitzontal sobre el pla
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        // Move the Skeleton to the castle.
        transform.position += direction * speed * Time.deltaTime;
    }

    // Hit the castle and die.
    private void OnTriggerEnter(Collider other)
    {
        CastleBehavior castle = other.GetComponentInParent<CastleBehavior>();

        if (castle != null)
        {
            castle.TakeDamage(damage);
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}