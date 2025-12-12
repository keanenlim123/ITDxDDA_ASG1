using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
using TMPro;
using Firebase.Auth;
using System;

public class DatabaseController : MonoBehaviour
{

    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    public TMP_InputField UsernameInput;

    [SerializeField] public GameObject SignUpCanvas;
    [SerializeField] public GameObject MainCanvas;

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

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
                { "Arctic",     new string[] { "Penguin", "Polar Bear" } },
                { "Mangrovoe",   new string[] { "Crocodile", "Dugong" } },
                { "Coral Reef", new string[] { "Clownfish", "Manta Ray" } }
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

    // Auth event handling example
    private void OnAuthStateChanged(object sender, EventArgs e)
    {
        Debug.Log("Auth state changed!");

        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.Log("User is not logged in!");
        }
        else
        {
            Debug.Log($"User logged in: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}");
        }
    }

    void Start()
    {
        // Auth event handling example
        FirebaseAuth.DefaultInstance.StateChanged += OnAuthStateChanged;


        var db = FirebaseDatabase.DefaultInstance.RootReference;

        // Change a single value using SetValueAsync
        db.Child("student").Child("detach8").Child("score").SetValueAsync(9999);

        // Update using UpdateChildrenAsync
        // This is used to bulk update multiple values
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["name"] = "Some awesome guy";
        data["score"] = 1234;
        db.Child("student").Child("detach8").UpdateChildrenAsync(data);

        // Delete the player "detach8"
        db.Child("student").Child("detach8").RemoveValueAsync();

        // Retrieve
        var retrieveTask = db.Child("student").Child("somenonexistentplayer").GetValueAsync();

        retrieveTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Error loading player!");
                return;
            }

            if (task.IsCompleted)
            {
                if (!task.Result.Exists)
                {
                    Debug.Log("Invalid player id!");
                    return;
                }

                string json = task.Result.GetRawJsonValue();
                Debug.Log(json);

                // Deserialize JSON data back to Player object
                Player p = JsonUtility.FromJson<Player>(json);
                Debug.Log($"Player loaded: {p.username}");
            }
        });

        Debug.Log("done");

    }

    // Update is called once per frame
    void Update()
    {

    }
}

