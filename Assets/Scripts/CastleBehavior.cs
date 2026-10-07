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

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.RegisterCastle(this);
            GameUIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            GameUIManager.Instance.UpdateWaveInfo(round, numberOfRounds, spawnAmout);
        }
    }

    private void Update()
    {
        if (ended) return;

        int enemiesToSpawn = spawnAmout - spawnCounter;
        if (enemiesText != null)
        {
            enemiesText.text = enemiesToSpawn + " Enemics";
        }

        if (waitingForEnemies)
        {
            int enemiesNum = FindObjectsByType<SkeletonBehavior>().Length;
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.UpdateWaveInfo(round, numberOfRounds, enemiesNum);
            }

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
                    if (roundText != null) roundText.text = "Ronda " + round;
                    waitingForEnemies = false;
                    if (waitText != null) waitText.gameObject.SetActive(false);
                    int spawnAugment = (int)((float)spawnAmout * difficultIncrease);
                    spawnAmout += spawnAugment;
                    spawnTime *= 1 - difficultIncrease;

                    if (GameUIManager.Instance != null)
                    {
                        GameUIManager.Instance.UpdateWaveInfo(round, numberOfRounds, spawnAmout);
                    }
                }
            }
        }
        else
        {
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.UpdateWaveInfo(round, numberOfRounds, enemiesToSpawn);
            }

            timer += Time.deltaTime;
            if (timer >= spawnTime)
            {
                if (spawner != null)
                {
                    spawner.SpawnSkeletonAroundCastle();
                }
                spawnCounter++;
                timer = 0f;

                if (spawnCounter >= spawnAmout)
                {
                    waitingForEnemies = true;
                    if (waitText != null) waitText.gameObject.SetActive(true);
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

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (ended) return;

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
            if (healthText != null) healthText.gameObject.SetActive(false);
            if (roundText != null) roundText.gameObject.SetActive(false);
            if (enemiesText != null) enemiesText.gameObject.SetActive(false);
            if (waitText != null)
            {
                waitText.text = "El castell ha caigut!";
                waitText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (healthText != null) healthText.gameObject.SetActive(false);
            if (roundText != null) roundText.gameObject.SetActive(false);
            if (enemiesText != null) enemiesText.gameObject.SetActive(false);
            if (waitText != null)
            {
                waitText.text = "Has guanyat!";
                waitText.gameObject.SetActive(true);
            }
        }

        // Neteja els esquelets restants en acabar la partida
        SkeletonBehavior[] enemies = FindObjectsByType<SkeletonBehavior>();
        foreach (var enemy in enemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowGameOver(!dead, round, numberOfRounds);
        }
        else
        {
            Debug.LogWarning("[CastleBehavior] GameUIManager.Instance es NULL en End()!");
        }
    }

    public void ResetCastle()
    {
        currentHealth = maxHealth;
        round = 1;
        timer = 0f;
        spawnCounter = 0;
        waitingForEnemies = false;
        ended = false;

        UpdateHealthDisplay();

        if (healthText != null) healthText.gameObject.SetActive(true);
        if (roundText != null)
        {
            roundText.text = "Ronda 1";
            roundText.gameObject.SetActive(true);
        }
        if (enemiesText != null)
        {
            enemiesText.text = spawnAmout + " Enemics";
            enemiesText.gameObject.SetActive(true);
        }
        if (waitText != null)
        {
            waitText.gameObject.SetActive(false);
        }

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            GameUIManager.Instance.UpdateWaveInfo(round, numberOfRounds, spawnAmout);
        }
    }
}
