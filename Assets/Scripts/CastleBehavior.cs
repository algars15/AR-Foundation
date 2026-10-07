using UnityEngine;
using TMPro;

public class CastleBehavior : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int spawnAmout = 5;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private float difficultIncrease = 0.2f;
    [SerializeField] private int numberOfRounds = 5;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI waitText;
    [SerializeField] private TextMeshProUGUI enemiesText;

    private int round = 1;
    private float spawnTime = 2;
    private float timer = 2;
    private int spawnCounter = 0;
    private EnemySpawner spawner;
    bool waitingForEnemies = false;
    bool ended = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        spawner = GetComponentInChildren<EnemySpawner>();
    }

    private void Update()
    {
        if (ended) return;

        int enemiesToSpawn = spawnAmout - spawnCounter;
        enemiesText.text = enemiesToSpawn + "Enemies";

        if (waitingForEnemies)
        {
            int enemiesNum = FindObjectsByType<SkeletonBehavior>().Length;
            if (enemiesNum == 0)
            {
                round++;
                if (round > numberOfRounds)
                {
                    End(false);
                }
                else
                {
                    spawnCounter = 0;
                    roundText.text = "Round" + round;
                    waitingForEnemies = false;
                    waitText.gameObject.SetActive(false);
                    int spawnAugment = (int)((float)spawnAmout * difficultIncrease);
                    spawnAmout += spawnAugment;
                    spawnTime *= 1 - difficultIncrease;
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= spawnTime)
            {
                spawner.SpawnSkeletonAroundCastle();
                spawnCounter++;
                timer = 0f;

                if (spawnCounter >= spawnAmout)
                {
                    waitingForEnemies = true;
                    waitText.gameObject.SetActive(true);
                }
            }
        }
    }

    public void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            End(true);
        }
    }


    public void End(bool dead)
    {
        ended = true;
        if (dead)
        {
            healthText.gameObject.SetActive(false);
            roundText.gameObject.SetActive(false);
            enemiesText.gameObject.SetActive(false);
            waitText.text = "The Castle has fallen!";
            waitText.gameObject.SetActive(true);
        }
        else
        {
            healthText.gameObject.SetActive(false);
            roundText.gameObject.SetActive(false);
            enemiesText.gameObject.SetActive(false);
            waitText.text = "You Won!";
            waitText.gameObject.SetActive(true);
        }
    }
}
