using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
using TMPro;
using Firebase.Auth;

/// <summary>
/// Handles Firebase authentication and database operations for players.
/// Supports signing up, signing in, signing out, and uploading initial player data including habitats and animals.
/// </summary>
public class DatabaseController : MonoBehaviour
{
    /// <summary>
    /// Input field for user email during sign up or sign in.
    /// </summary>
    public TMP_InputField EmailInput;

    /// <summary>
    /// Input field for user password during sign up or sign in.
    /// </summary>
    public TMP_InputField PasswordInput;

    /// <summary>
    /// Input field for username during sign up.
    /// </summary>
    public TMP_InputField UsernameInput;

    /// <summary>
    /// UI Canvas displayed for sign up.
    /// </summary>
    [SerializeField] public GameObject SignUpCanvas;

    /// <summary>
    /// Main UI Canvas displayed after successful login or sign up.
    /// </summary>
    [SerializeField] public GameObject MainCanvas;

    /// <summary>
    /// Signs the current user out from Firebase authentication.
    /// </summary>
    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    /// <summary>
    /// Creates a new user with email and password, sets up the initial player data including habitats and animals,
    /// and uploads it to Firebase Realtime Database.
    /// </summary>
    public void SignUp()
    {
        var signupTask = FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);

        signupTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Can't sign up due to error.");
                return;
            }

            if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                string uid = newUser.UserId;

                Debug.Log($"User signed up successfully, id: {uid}");

                // --- Create Player Base Data ---
                Dictionary<string, object> playerData = new Dictionary<string, object>();
                playerData["username"] = UsernameInput.text;
                playerData["email"] = EmailInput.text;
                playerData["password"] = PasswordInput.text;

                // --- Habitat and Animals Structure ---
                Dictionary<string, object> habitats = new Dictionary<string, object>();

                // Define each habitat and its animals
                Dictionary<string, string[]> habitatAnimals = new Dictionary<string, string[]>()
                {
                    { "Ocean",      new string[] { "Jellyfish", "Sunfish" } },
                    { "Arctic",     new string[] { "Penguin", "Polarbear" } },
                    { "Mangroove",  new string[] { "Crocodile", "Frog" } },
                    { "Coral Reef", new string[] { "Clownfish", "Mantaray" } }
                };

                foreach (var habitat in habitatAnimals)
                {
                    Dictionary<string, object> animalEntries = new Dictionary<string, object>();

                    foreach (string animal in habitat.Value)
                    {
                        Dictionary<string, object> stats = new Dictionary<string, object>();
                        stats["timeTaken"] = 0f;
                        stats["pointsEarned"] = 0;

                        animalEntries[animal] = stats;
                    }

                    habitats[habitat.Key] = animalEntries;
                }

                // Add to player data
                playerData["habitats"] = habitats;

                // Upload to Firebase
                DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;
                db.Child("players").Child(uid).SetValueAsync(playerData)
                    .ContinueWithOnMainThread(uploadTask =>
                    {
                        if (uploadTask.IsCompleted)
                        {
                            Debug.Log("Player data uploaded successfully.");

                            SignUpCanvas.SetActive(false);
                            MainCanvas.SetActive(true);
                        }
                        else
                        {
                            Debug.Log("Failed to upload player data.");
                        }
                    });
            }
        });
    }

    /// <summary>
    /// Signs in an existing user using email and password.
    /// Logs the user id upon successful login.
    /// </summary>
    public void SignIn()
    {
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Can't sign in due to error!!!");
            }

            if (task.IsCompleted)
            {
                Debug.Log($"User logged in, id: {task.Result.User.UserId}");

                // Code to load the user profile
            }
        });

        Debug.Log("Hahahahaha");
    }
}
