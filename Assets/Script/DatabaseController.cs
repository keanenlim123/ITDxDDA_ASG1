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
    void Start()
    {
        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;

        Dictionary<string, object> animalsData = new Dictionary<string, object>()
    {
        {
            "Ocean", new Dictionary<string, object>()
            {
                { "Jellyfish", new Dictionary<string, object>()
                    {
                        { "name", "Jellyfish" },
                        { "description", "Jellyfish are soft, floating sea animals with umbrella-shaped bodies and long tentacles. They move by gently pulsing through the water and use their tentacles to catch food and defend themselves. Jellyfish can be found in oceans all over the world." }
                    }
                },
                { "Sunfish", new Dictionary<string, object>()
                    {
                        { "name", "Sunfish" },
                        { "description", "Sunfish, also called ocean sunfish, are the heaviest bony fish in the world. They have flat, round bodies and are often seen near the ocean’s surface, where they bask in the sun to warm up. Sunfish mainly eat jellyfish and live in temperate and tropical oceans." }
                    }
                }
            }
        },
        {
            "Arctic", new Dictionary<string, object>()
            {
                { "Penguin", new Dictionary<string, object>()
                    {
                        { "name", "Penguin" },
                        { "description", "Penguins are flightless birds that live mostly in the Southern Hemisphere. They are excellent swimmers and use their strong flippers to glide through the water while hunting for fish, squid, and krill. Penguins have thick feathers and a layer of fat to keep them warm in cold environments, especially in Antarctica." }
                    }
                },
                { "Polarbear", new Dictionary<string, object>()
                    {
                        { "name", "Polarbear" },
                        { "description", "Polar bears are large, powerful mammals that live in the Arctic. They have thick white fur and a layer of fat that helps them stay warm in freezing temperatures. Polar bears are excellent swimmers and mainly hunt seals on sea ice for food." }
                    }
                }
            }
        },
        {
            "Mangroove", new Dictionary<string, object>()
            {
                { "Crocodile", new Dictionary<string, object>()
                    {
                        { "name", "Crocodile" },
                        { "description", "Crocodiles are large reptiles that live in rivers, lakes, and wetlands. They have powerful jaws, tough skin, and are excellent swimmers. Crocodiles are carnivores and can stay underwater for long periods while hunting." }
                    }
                },
                { "Frog", new Dictionary<string, object>()
                    {
                        { "name", "Frog" },
                        { "description", "Frogs are small amphibians that live both on land and in water. They have smooth skin, strong back legs for jumping, and they start life as tadpoles before growing into adults." }
                    }
                }
            }
        },
        {
            "Coral Reef", new Dictionary<string, object>()
            {
                { "Clownfish", new Dictionary<string, object>()
                    {
                        { "name", "Clownfish" },
                        { "description", "Clownfish are small, brightly colored fish that live in warm ocean waters. They are famous for living among sea anemones, which protect them from predators. In return, clownfish help keep the anemone clean and safe." }
                    }
                },
                { "Mantaray", new Dictionary<string, object>()
                    {
                        { "name", "Manta Ray" },
                        { "description", "Manta rays are large, gentle ocean animals with wide, flat bodies and long fins that look like wings. They glide gracefully through the water and feed by filtering tiny plankton from the sea. Manta rays are often found in tropical and subtropical oceans." }
                    }
                }
            }
        }
    };

        db.Child("animals").SetValueAsync(animalsData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Animals database created successfully.");
            }
            else
            {
                Debug.Log("Failed to create animals database.");
            }
        });
    }
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
