using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{
    [Header("Prefab a col·locar")]
    [SerializeField] private GameObject objectToPlace;

    private ARRaycastManager raycastManager;
    private GameObject spawnedObject;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // 1. Comprovar si l'usuari està tocant la pantalla
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // Només volem detectar l'inici del toc
        if (touch.phase != TouchPhase.Began) return;

        // 2. Fer Raycast des del punt del toc cap al món real
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Puntuació/Posició d'impacte sobre el pla detectat
            Pose hitPose = hits[0].pose;

            // 3. Si l'objecte no s'ha creat encara, l'instanciem
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
            }
            else
            {
                // Si ja existeix, el movem a la nova posició
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }

    // Mètode públic per poder reiniciar la col·locació des d'un botó de Reset
    public void ResetPlacement()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }
}