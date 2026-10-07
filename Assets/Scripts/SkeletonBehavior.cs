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

        Vector3 direction = (targetCastle.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        transform.position += direction * speed * Time.deltaTime;
    }

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