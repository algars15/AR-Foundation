using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Economia")]
    [SerializeField] private int currentCoins = 10;
    [SerializeField] private int cannonCost = 5;

    [Header("Límit de Canons")]
    [SerializeField] private int maxCannons = 3;
    private int currentCannonsCount = 0;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    public bool CanSpawnCannon()
    {
        if (currentCannonsCount >= maxCannons)
        {
            Debug.Log("S'ha assolit el límit màxim de canons!");
            return false;
        }

        if (currentCoins < cannonCost)
        {
            Debug.Log("No tens suficients monedes!");
            return false;
        }

        return true;
    }

    public void RegisterCannonSpawned()
    {
        currentCoins -= cannonCost;
        currentCannonsCount++;
        UpdateUI();
    }

    public void RegisterCannonDestroyed()
    {
        if (currentCannonsCount > 0)
        {
            currentCannonsCount--;
            UpdateUI();
        }
    }

    public void ResetEconomy()
    {
        currentCoins = 10;
        currentCannonsCount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Monedes: {currentCoins} | Canons: {currentCannonsCount}/{maxCannons}";
        }
    }
}