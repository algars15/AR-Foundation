using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; //[cite: 5]
    [SerializeField] private float lifeTime = 4f; //[cite: 5]

    private void Start()
    {
        Destroy(gameObject, lifeTime); //[cite: 5]
    }

    private void Update()
    {
        // Avança cap endavant (eix Z del prefab de la bala)
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        SkeletonBehavior skeleton = other.GetComponentInParent<SkeletonBehavior>(); //[cite: 5]

        if (skeleton != null)
        {
            Destroy(skeleton.gameObject); // Destrueix l'esquelet[cite: 5]
            Destroy(gameObject);          // Destrueix la bala[cite: 5]
        }
    }
}