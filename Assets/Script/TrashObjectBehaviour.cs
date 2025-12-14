using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Handles the behavior of a trash object in the scene, including floating motion,
/// collision detection with a tong, and playing a cleaning sound.
/// </summary>
public class TrashObjectBehaviour : MonoBehaviour
{
    [Header("Floating Settings")]

    /// <summary>
    /// Speed of the vertical floating motion.
    /// </summary>
    [SerializeField] private float floatSpeed = 1f;

    /// <summary>
    /// Maximum height the object floats above/below its starting position.
    /// </summary>
    [SerializeField] private float floatHeight = 0.15f;

    [Header("Sound Effect")]

    /// <summary>
    /// Sound effect to play when the trash is cleaned.
    /// </summary>
    [SerializeField] private AudioClip cleanSFX;

    /// <summary>
    /// AudioSource used to play the cleaning sound.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Locked world position to prevent camera movement.
    /// </summary>
    private Vector3 lockedWorldPosition;

    /// <summary>
    /// Reference to the parent habitat this trash belongs to.
    /// </summary>
    public HabitatController parentHabitat;

    /// <summary>
    /// Initializes the trash object, locks world position,
    /// and ensures required components exist.
    /// </summary>
    private void Start()
    {
        // Lock the world position ONCE
        lockedWorldPosition = transform.position;

        // Add AR Anchor for stability
        if (GetComponent<ARAnchor>() == null)
        {
            gameObject.AddComponent<ARAnchor>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Applies floating motion after AR camera updates.
    /// </summary>
    private void LateUpdate()
    {
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            lockedWorldPosition.x,
            lockedWorldPosition.y + floatOffset,
            lockedWorldPosition.z
        );
    }

    /// <summary>
    /// Handles collision with objects tagged as "Tong".
    /// Cleans the trash, plays a sound, and destroys both objects.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Tong"))
            return;

        // Notify parent habitat
        if (parentHabitat != null)
        {
            parentHabitat.CleanTrash();
        }

        // Play sound
        audioSource.PlayOneShot(cleanSFX);

        // Destroy objects
        Destroy(gameObject, 0.1f);
        Destroy(other.gameObject);
    }
}
