using UnityEngine;

public class SpawnFromUIButton : MonoBehaviour
{
    public GameObject prefabToSpawn;   // Your sponge prefab
    public Canvas parentCanvas;        // The canvas your button is inside
    public Camera arCamera;            // AR Camera

    public void SpawnPrefabAtButton()
    {
        // 1. Get the button's RectTransform
        RectTransform buttonRect = GetComponent<RectTransform>();

        // 2. Convert UI position to screen coords
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, buttonRect.position);

        // 3. Convert screen coords to AR world space (slightly forward from camera)
        Ray ray = arCamera.ScreenPointToRay(screenPos);

        Vector3 spawnWorldPos = ray.GetPoint(0.3f); // 0.3m in front of camera (adjust if needed)

        // 4. Spawn the prefab
        Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);
    }
}
