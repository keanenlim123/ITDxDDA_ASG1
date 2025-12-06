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
            }
        }
    }
}
