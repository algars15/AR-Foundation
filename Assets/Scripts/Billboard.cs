using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Orienta el texto hacia la cámara del dispositivo
            transform.LookAt(transform.position + mainCamera.forward);
        }
    }
}