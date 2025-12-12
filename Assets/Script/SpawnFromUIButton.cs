using UnityEngine;

/// <summary>
/// Spawns a prefab in AR space relative to a UI button's position when the button is clicked.
/// Useful for dragging objects from UI into the AR scene.
/// </summary>
public class SpawnFromUIButton : MonoBehaviour
{
    /// <summary>
    /// The prefab to spawn (e.g., a sponge prefab).
    /// </summary>
    public GameObject prefabToSpawn;

    /// <summary>
    /// The Canvas that contains this button.
    /// Used to convert UI positions to screen coordinates.
    /// </summary>
    public Canvas parentCanvas;

    /// <summary>
    /// The AR Camera used to convert screen coordinates to world space.
    /// </summary>
    public Camera arCamera;

    /// <summary>
    /// Spawns the assigned prefab at a position in front of the AR camera
    /// corresponding to the button's position in the UI.
    /// </summary>
    public void SpawnPrefabAtButton()
    {
        // 1. Get the button's RectTransform
        RectTransform buttonRect = GetComponent<RectTransform>();

        // 2. Convert UI position to screen coordinates
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, buttonRect.position);

        // 3. Convert screen coordinates to AR world space (slightly forward from camera)
        Ray ray = arCamera.ScreenPointToRay(screenPos);

        Vector3 spawnWorldPos = ray.GetPoint(0.3f); // 0.3m in front of camera (adjust if needed)

        // 4. Spawn the prefab at the computed world position
        Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);
    }
}
