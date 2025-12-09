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
    public void Feed()
    {
        currentHealth += 50;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        // When habitat reaches full health, activate fishes
        if (currentHealth == maxHealth)
        {
            foreach (GameObject fish in fishes)
            {
                fish.SetActive(true);
            }
        }

        healthBar.value = currentHealth;

        Debug.Log(habitatName + " fed! Health now: " + currentHealth);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Feedbag"))
        {
            // Only allow feeding if currentHealth is more than 50
            if (currentHealth >= 50)
            {
                Feed();
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log(habitatName + " health too low (< 50). Cannot feed yet.");
            }
        }
    }
}
