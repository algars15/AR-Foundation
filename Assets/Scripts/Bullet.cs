using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 4f; // Destrucció automàtica si no impacta

    private void Start()
    {
        // Allibera memòria si la bala surt disparada i no toca res
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Avança en la direcció cap a on apunta el prefab
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprovem si ha impactat contra un esquelet (o un fill del seu prefab)
        SkeletonBehavior skeleton = other.GetComponentInParent<SkeletonBehavior>();

        if (skeleton != null)
        {
            Destroy(skeleton.gameObject); // Elimina l'esquelet a l'instant
            Destroy(gameObject);          // Destrueix la bala
        }
    }
}