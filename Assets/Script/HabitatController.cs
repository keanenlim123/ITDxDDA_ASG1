using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of a habitat, including health management, feeding, cleaning trash, 
/// and activating associated fish objects. Also handles UI and audio feedback.
/// </summary>
public class HabitatController : MonoBehaviour
{
    [Header("Habitat Settings")]

    /// <summary>
    /// Name of the habitat.
    /// </summary>
    public string habitatName;

    /// <summary>
    /// Maximum health of the habitat.
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// Current health of the habitat.
    /// </summary>
    public int currentHealth = 0;

    /// <summary>
    /// Minimum health required before feeding can occur.
    /// </summary>
    public int requiredHealth = 50;

    /// <summary>
    /// List of fish GameObjects associated with this habitat. They are activated when habitat is fully healed.
    /// </summary>
    public List<GameObject> fishes = new List<GameObject>();

    [Header("UI")]

    /// <summary>
    /// Slider representing the habitat's health in the UI.
    /// </summary>
    public Slider healthBar;

    [Header("Audio")]

    /// <summary>
    /// Audio clip played when the habitat is fed.
    /// </summary>
    public AudioClip feedingSound;

    /// <summary>
    /// Initializes the health bar at the start of the scene.
    /// </summary>
    void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Make the slider non-interactable
        healthBar.interactable = false;
    }

    /// <summary>
    /// Cleans trash in the habitat, increasing health by 10.
    /// Activates fish if the habitat reaches max health.
    /// </summary>
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

    /// <summary>
    /// Feeds the habitat, increasing health by 50.
    /// Plays a feeding sound and activates fish if health reaches maximum.
    /// </summary>
    public void Feed()
    {
        PlayFeedSound();

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

    /// <summary>
    /// Detects collisions with feed items. Only allows feeding if current health is above the required threshold.
    /// </summary>
    /// <param name="other">The collider that enters the trigger.</param>
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

    /// <summary>
    /// Plays the assigned feeding sound at the habitat's position.
    /// </summary>
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
