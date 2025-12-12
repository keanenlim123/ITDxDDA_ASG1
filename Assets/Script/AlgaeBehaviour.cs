using UnityEngine;

/// <summary>
/// Handles the behavior of a trash object in the scene, including floating motion,
/// collision detection with a sponge, and playing a cleaning sound.
/// </summary>
public class AlgaeBehaviour : MonoBehaviour
{
    [Header("Floating Settings")]

    /// <summary>
    /// Speed of the vertical floating motion.
    /// </summary>
    [SerializeField] private float floatSpeed = 0.05f;

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
    /// Starting position of the trash object, used for floating calculation.
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// Reference to the parent habitat this trash belongs to.
    /// </summary>
    public HabitatController parentHabitat;

    /// <summary>
    /// Initializes the trash object and ensures an AudioSource is attached.
    /// </summary>
    private void Start()
    {
        startPos = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Updates the vertical position of the trash object to create a floating effect.
    /// </summary>
    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// <summary>
    /// Handles collision with objects tagged as "Sponge".
    /// Cleans the trash, plays a sound, and destroys both objects.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sponge"))
        {
            // Notify parent habitat to clean
            if (parentHabitat != null)
            {
                parentHabitat.CleanTrash();
            }

            // Play cleaning sound effect
            audioSource.PlayOneShot(cleanSFX);

            // Destroy this trash object and the sponge
            Destroy(gameObject, 0.1f);
            Destroy(other.gameObject);
        }
    }
}
