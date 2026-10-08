using UnityEngine;

public class CannonSpawnValidator : MonoBehaviour
{
    private void Awake()
    {
        if (CoinManager.Instance != null)
        {
            if (CoinManager.Instance.CanSpawnCannon())
            {
                CoinManager.Instance.RegisterCannonSpawned();
            }
            else
            {
                Debug.Log("Límit de canons o monedes insuficients! Cancellant instància.");
                Destroy(gameObject);
            }
        }
    }
}