using UnityEngine;
using UnityEngine.UI;

public class HabitatController : MonoBehaviour
{
    [Header("Habitat Settings")]
    public string habitatName;
    public int maxHealth = 100;
    public int currentHealth = 0;

    [Header("UI")]
    public Slider healthBar;

    void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void CleanTrash()
    {
        currentHealth += 10;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.value = currentHealth;

        Debug.Log(habitatName + " health increased! Now: " + currentHealth);
    }
}
