using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitatController : MonoBehaviour
{
    [Header("Habitat Settings")]
    public string habitatName;
    public int maxHealth = 100;
    public int currentHealth = 0;
    public List<GameObject> fishes = new List<GameObject>();

    [Header("UI")]
    public Slider healthBar;

    void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        healthBar.interactable = false;
    }

    public void CleanTrash()
    {
        currentHealth += 10;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth == 100)
        {
            foreach (GameObject fish in fishes)
            {
                fish.SetActive(true);
            }
        }

        healthBar.value = currentHealth;

        Debug.Log(habitatName + " health increased! Now: " + currentHealth);

    }
}
