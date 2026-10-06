using UnityEngine;

public class CannonBehavior : MonoBehaviour
{
    [Header("Prefabs i Referències")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("Objecte fill que indica exactament el punt de sortida de la bala")]
    [SerializeField] private Transform firePoint;

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
        if (bulletPrefab == null || firePoint == null) return;

        // La bala neix exactament a la posició i rotació del FirePoint
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    // Dibuixa una petita esfera groga a la vista de Scene a la posició del FirePoint
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firePoint.position, 0.03f);
        }
    }
}