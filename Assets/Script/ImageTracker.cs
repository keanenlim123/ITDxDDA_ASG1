using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages AR image tracking, spawning prefabs for tracked images,
/// and displaying associated UI canvases based on the tracked prefab's tag.
/// </summary>
public class ImageTracker : MonoBehaviour
{
    /// <summary>
    /// ARTrackedImageManager responsible for handling image tracking events.
    /// </summary>
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    /// <summary>
    /// Prefabs that can be placed in AR when their corresponding reference images are detected.
    /// </summary>
    [SerializeField]
    private GameObject[] placeablePrefabs;

    /// <summary>
    /// Dictionary mapping prefab names to their spawned instances in the scene.
    /// </summary>
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    [Header("UI Canvases")]

    /// <summary>
    /// Canvas to show when an Arctic prefab is tracked.
    /// </summary>
    [SerializeField] private GameObject arcticCanvas;

    /// <summary>
    /// Canvas to show when an Ocean prefab is tracked.
    /// </summary>
    [SerializeField] private GameObject oceanCanvas;

    /// <summary>
    /// Canvas to show when a Mangrove prefab is tracked.
    /// </summary>
    [SerializeField] private GameObject mangroveCanvas;

    /// <summary>
    /// Canvas to show when a Coral Reef prefab is tracked.
    /// </summary>
    [SerializeField] private GameObject coralReefCanvas;

    /// <summary>
    /// Initializes the ARTrackedImageManager and spawns prefabs at the start of the scene.
    /// </summary>
    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    /// <summary>
    /// Instantiates all placeable prefabs at the origin and deactivates them initially.
    /// </summary>
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

    /// <summary>
    /// Called whenever tracked images are added, updated, or removed.
    /// </summary>
    /// <param name="eventArgs">AR trackable changes event arguments.</param>
    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
            UpdateImage(trackedImage);

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
            UpdateImage(trackedImage);

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
            UpdateImage(lostObj.Value);
    }

    /// <summary>
    /// Updates a single tracked image by activating/deactivating its prefab and showing the appropriate canvas.
    /// </summary>
    /// <param name="trackedImage">The ARTrackedImage to update.</param>
    void UpdateImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

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

    /// <summary>
    /// Hides all UI canvases.
    /// </summary>
    void HideAllCanvases()
    {
        arcticCanvas.SetActive(false);
        oceanCanvas.SetActive(false);
        mangroveCanvas.SetActive(false);
        coralReefCanvas.SetActive(false);
    }

    /// <summary>
    /// Shows the appropriate canvas based on the prefab's tag.
    /// </summary>
    /// <param name="prefabTag">The tag of the tracked prefab.</param>
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

    /// <summary>
    /// Updates every frame to ensure only the correct canvas is active
    /// based on the currently active spawned prefab.
    /// </summary>
    void Update()
    {
        foreach (var kvp in spawnedPrefabs)
        {
            GameObject prefab = kvp.Value;

            if (prefab.activeInHierarchy)
            {
                ShowCanvasBasedOnTag(prefab.tag);
                return; // Only show 1 canvas at a time
            }
        }

        HideAllCanvases();
    }
}
