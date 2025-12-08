using UnityEngine;

public class TrashObject : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 0.05f;     // Speed of up/down motion
    [SerializeField] private float floatHeight = 0.15f; // Distance of movement

    private Vector3 startPos;
    public HabitatController parentHabitat;

    private void Start()
    {
        startPos = transform.position;
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

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
