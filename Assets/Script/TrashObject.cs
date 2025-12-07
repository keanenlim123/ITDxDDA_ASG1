using UnityEngine;

public class TrashObject : MonoBehaviour
{
    public HabitatController parentHabitat;

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
