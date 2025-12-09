using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject arcticCanvas;
    [SerializeField] private GameObject oceanCanvas;
    [SerializeField] private GameObject mangroveCanvas;

    [SerializeField] private GameObject coralReefCanvas;

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if (trackedImage != null)
        {
            string imageName = trackedImage.referenceImage.name;

            if (trackedImage.trackingState == TrackingState.Limited ||
                trackedImage.trackingState == TrackingState.None)
            {
                spawnedPrefabs[imageName].transform.SetParent(null);
                spawnedPrefabs[imageName].SetActive(false);
                return;
            }

            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                GameObject prefab = spawnedPrefabs[imageName];

                // Attach prefab to tracked image
                if (prefab.transform.parent != trackedImage.transform)
                {
                    prefab.transform.SetParent(trackedImage.transform);
                    prefab.SetActive(true);
                }

                float distanceInFront = 0.1f;
                prefab.transform.localPosition = trackedImage.transform.up * distanceInFront;

                prefab.transform.localRotation = Quaternion.identity;

                ShowCanvasBasedOnTag(prefab.tag);
            }
        }
    }

    void HideAllCanvases()
    {
        arcticCanvas.SetActive(false);
        oceanCanvas.SetActive(false);
        mangroveCanvas.SetActive(false);
        coralReefCanvas.SetActive(false);
    }

    void ShowCanvasBasedOnTag(string prefabTag)
    {
        HideAllCanvases();

        if (prefabTag == "Artic")
            arcticCanvas.SetActive(true);
        else if (prefabTag == "Ocean")
            oceanCanvas.SetActive(true);
        else if (prefabTag == "Mangroove")
            mangroveCanvas.SetActive(true);
        else if (prefabTag == "CoralReef")
            coralReefCanvas.SetActive(true);
    }

    void Update()
    {
        // Loop through all spawned prefabs
        foreach (var kvp in spawnedPrefabs)
        {
            GameObject prefab = kvp.Value;

            // Only check active prefabs (currently being tracked)
            if (prefab.activeInHierarchy)
            {
                // Show canvas based on prefab tag
                ShowCanvasBasedOnTag(prefab.tag);

                // Optional: break if you only want 1 canvas visible at a time
                return;
            }
        }

        // If no prefab is active, hide all canvases
        HideAllCanvases();
    }

}
