using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;

/// <summary>
/// Represents the behaviour and data for an animal in the game.
/// Handles loading animal data from Firebase and storing quiz information.
/// </summary>
public class AnimalBehaviour : MonoBehaviour
{
    [Header("Animal Info")]

    /// <summary>
    /// The name of the animal.
    /// </summary>
    public string animalName;

    /// <summary>
    /// A description of the animal. Can span multiple lines.
    /// </summary>
    [TextArea]
    public string description;

    /// <summary>
    /// The name of the habitat where this animal can be found.
    /// </summary>
    public string habitatName;

    /// <summary>
    /// The texture or image representing this animal.
    /// </summary>
    public Texture animalImage;

    [Header("Quiz Data")]

    /// <summary>
    /// The list of quiz questions related to this animal.
    /// Should have the same size as <see cref="answers"/> and <see cref="correctAnswerIndexes"/>.
    /// </summary>
    public List<string> questions;

    /// <summary>
    /// The list of possible answers for each question.
    /// Each <see cref="AnswerSet"/> should contain exactly 3 answers.
    /// </summary>
    public List<AnswerSet> answers;

    /// <summary>
    /// The index of the correct answer for each question.
    /// Should match the number of questions. Values are 0, 1, or 2.
    /// </summary>
    public List<int> correctAnswerIndexes;

    /// <summary>
    /// Unity's Start method. Called on the frame when a script is enabled.
    /// Initiates loading of animal data from Firebase.
    /// </summary>

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase initialized successfully");
                LoadAnimalFromFirebase(); // Move here
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + task.Result);
            }
        });
    }

    /// <summary>
    /// Loads the animal's data from Firebase Realtime Database.
    /// Populates <see cref="animalName"/> and <see cref="description"/> fields.
    /// </summary>
    void LoadAnimalFromFirebase()
    {
        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;

        db.Child("animals")
          .Child(habitatName)
          .Child(animalName)
          .GetValueAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.LogError($"Failed to load animal {animalName} in {habitatName}");
                  return;
              }

              DataSnapshot snapshot = task.Result;
              if (!snapshot.Exists)
              {
                  Debug.LogError($"Animal not found in Firebase: {animalName}");
                  return;
              }

              animalName = snapshot.Child("name").Value.ToString();
              description = snapshot.Child("description").Value.ToString();

              Debug.Log($"Loaded animal from Firebase: {animalName}");
          });
    }
}

/// <summary>
/// Represents a set of possible answers for a quiz question.
/// </summary>
[System.Serializable]
public class AnswerSet
{
    /// <summary>
    /// The list of possible answers.
    /// </summary>
    public List<string> answers;
}
