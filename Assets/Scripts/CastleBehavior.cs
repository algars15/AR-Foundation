using UnityEngine;
using TMPro;

public class CastleBehavior : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            // Muestra "Vida: 100 / 100" (sin decimales)
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Evita que baje de 0
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            Debug.Log("¡The Castle has fallen!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
