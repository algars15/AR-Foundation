using UnityEngine;

public class CannonBehavior : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0.2f);

    [Header("Cadència de tir")]
    [SerializeField] private float fireRate = 1.5f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        // Calculem el punt de sortida combinant la posició amb els eixos locals del canó:
        // transform.right = eix X (direcció del canó)
        // transform.up = eix Y (alçada del canó)
        Vector3 spawnPos = transform.position
                         + (transform.right * spawnOffset.x)
                         + (transform.up * spawnOffset.y);

        Instantiate(bulletPrefab, spawnPos, transform.rotation);
    }
}