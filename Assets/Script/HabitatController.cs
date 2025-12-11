using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitatController : MonoBehaviour
{
    [Header("Habitat Settings")]
    public string habitatName;
    public int maxHealth = 100;
    public int currentHealth = 0;
    public int requiredHealth = 50;
    public List<GameObject> fishes = new List<GameObject>();

    [Header("UI")]
    public Slider healthBar;

    [Header("Audio")]
    public AudioClip feedingSound;

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
        else if (currentHealth == maxHealth)
        {
            foreach (GameObject fish in fishes)
                fish.SetActive(true);
        }

        healthBar.value = currentHealth;

        Debug.Log(habitatName + " health increased! Now: " + currentHealth);
    }

    public void Feed()
    {
        PlayFeedSound(); // 🔊 Play feeding sound

        currentHealth += 50;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth == maxHealth)
        {
            foreach (GameObject fish in fishes)
                fish.SetActive(true);
        }

        healthBar.value = currentHealth;

        Debug.Log(habitatName + " fed! Health now: " + currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Feedbag"))
        {
            if (currentHealth >= requiredHealth)
            {
                Feed();

                Destroy(other.gameObject, 0.3f);
            }
            else
            {
                Debug.Log(habitatName + " health too low. Cannot feed yet.");
            }
        }
    }

    private void PlayFeedSound()
    {
        if (feedingSound != null)
        {
            AudioSource.PlayClipAtPoint(feedingSound, transform.position, 1f);
        }
        else
        {
            Debug.LogWarning("No feedingSound assigned!");
        }
    }

}
