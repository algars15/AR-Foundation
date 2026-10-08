using UnityEngine;
using TMPro;

public class SkeletonBehavior : MonoBehaviour
{
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;

    [SerializeField] private string bulletTag = "Bullet";

    private Transform targetCastle;
    private Transform mainCameraTransform;

    [SerializeField] private TextMeshPro healthText;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Search the castle in the scene.
        CastleBehavior castle = FindAnyObjectByType<CastleBehavior>();
        if (castle != null)
        {
            targetCastle = castle.transform;
        }

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        UpdateHealthDisplay();

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

    private void LateUpdate()
    {
        if (healthText != null && mainCameraTransform != null)
        {
            healthText.transform.rotation = Quaternion.LookRotation(healthText.transform.position - mainCameraTransform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(bulletTag))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
            return;
        }

        CastleBehavior castle = other.GetComponentInParent<CastleBehavior>();

        if (castle != null)
        {
            castle.TakeDamage(damage);
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthDisplay();
        
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}