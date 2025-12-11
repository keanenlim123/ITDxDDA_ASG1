using UnityEngine;

public class TrashObjectBehaviour : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 0.05f;
    [SerializeField] private float floatHeight = 0.15f;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip cleanSFX;   // sound played when trash is cleaned
    private AudioSource audioSource;

    private Vector3 startPos;
    public HabitatController parentHabitat;

    private void Start()
    {
        startPos = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sponge"))
        {
            if (parentHabitat != null)
            {
                parentHabitat.CleanTrash();
            }

            audioSource.PlayOneShot(cleanSFX);


            Destroy(gameObject, 0.1f);
            Destroy(other.gameObject);
        }
    }
}
