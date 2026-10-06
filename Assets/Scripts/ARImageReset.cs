using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARImageReset : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    public void ResetCannon()
    {
        if (trackedImageManager == null)
            trackedImageManager = FindAnyObjectByType<ARTrackedImageManager>();

        if (trackedImageManager != null)
        {
            // Neteja els objectes instanciats per la imatge
            foreach (var trackedImage in trackedImageManager.trackables)
            {
                Destroy(trackedImage.gameObject);
            }

            // Reinicia el component per tornar a detectar el marcador
            trackedImageManager.enabled = false;
            trackedImageManager.enabled = true;
        }
    }
}