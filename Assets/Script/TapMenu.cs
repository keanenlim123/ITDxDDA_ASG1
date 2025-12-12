using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

/// <summary>
/// Handles the UI for interacting with animals, including learning information,
/// taking quizzes, and saving quiz results to Firebase.
/// </summary>
public class TapMenu : MonoBehaviour
{
    [Header("Panels")]

    /// <summary>
    /// Panel displaying options when an animal is tapped.
    /// </summary>
    public GameObject optionsMenuPanel;

    /// <summary>
    /// Panel displaying animal information (name, description, image).
    /// </summary>
    public GameObject animalInfoPanel;

    /// <summary>
    /// Panel displaying quiz questions and answers.
    /// </summary>
    public GameObject quizPanel;

    [Header("Buttons")]

    /// <summary>
    /// Button to cancel the options menu.
    /// </summary>
    public Button optionsCancelButton;

    /// <summary>
    /// Button to open the animal info panel.
    /// </summary>
    public Button learnButton;

    /// <summary>
    /// Button to start the quiz.
    /// </summary>
    public Button quizButton;

    /// <summary>
    /// Button to close the animal info panel.
    /// </summary>
    public Button animalInfoCancelButton;

    /// <summary>
    /// Button to cancel the quiz panel.
    /// </summary>
    public Button quizCancelButton;

    [Header("Animal Info UI")]

    /// <summary>
    /// Text field displaying the animal's name.
    /// </summary>
    public TMP_Text animalNameText;

    /// <summary>
    /// Text field displaying the animal's description.
    /// </summary>
    public TMP_Text descriptionText;

    /// <summary>
    /// Image displaying the animal.
    /// </summary>
    public RawImage animalImage;

    [Header("Quiz UI")]

    /// <summary>
    /// Text field displaying the current quiz question.
    /// </summary>
    public TMP_Text questionText;

    /// <summary>
    /// Array of buttons representing multiple-choice answers.
    /// </summary>
    public Button[] answerButtons; // Size = 3

    // Currently selected animal
    private AnimalBehaviour selectedAnimal;

    // Index of the current quiz question
    private int currentQuestionIndex = 0;

    /// <summary>
    /// Tracks if the quiz has already been completed by the user.
    /// </summary>
    private bool quizAlreadyCompleted = false;

    // Tracking quiz stats
    private float quizStartTime;
    private int pointsEarnedThisQuiz = 0;

    /// <summary>
    /// Current habitat of the selected animal (used for saving quiz results).
    /// </summary>
    private string currentHabitat = "Ocean";

    /// <summary>
    /// Initializes UI and button listeners.
    /// </summary>
    void Start()
    {
        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(false);

        optionsCancelButton.onClick.AddListener(CloseAllUI);
        animalInfoCancelButton.onClick.AddListener(CloseAllUI);
        quizCancelButton.onClick.AddListener(CloseAllUI);

        learnButton.onClick.AddListener(OpenAnimalInfo);
        quizButton.onClick.AddListener(StartQuiz);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    /// <summary>
    /// Detects clicks on animals in the scene and opens the options menu.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                AnimalBehaviour info = hit.collider.GetComponentInParent<AnimalBehaviour>();

                if (info != null)
                {
                    selectedAnimal = info;
                    currentQuestionIndex = 0;
                    currentHabitat = info.habitatName;

                    // Check Firebase before opening the menu
                    CheckIfQuizCompleted(selectedAnimal.animalName, currentHabitat);

                    optionsMenuPanel.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Checks Firebase to see if the quiz for the selected animal has already been completed.
    /// </summary>
    /// <param name="animalName">Name of the animal.</param>
    /// <param name="habitat">Name of the habitat.</param>
    void CheckIfQuizCompleted(string animalName, string habitat)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null) return;

        string uid = user.UserId;

        DatabaseReference animalRef = FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("players")
            .Child(uid)
            .Child("habitats")
            .Child(habitat)
            .Child(animalName);

        animalRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted) return;

            DataSnapshot s = task.Result;

            quizAlreadyCompleted = s.Child("isCompleted").Exists &&
                                   Convert.ToBoolean(s.Child("isCompleted").Value);

            // Disable quiz button if completed
            quizButton.interactable = !quizAlreadyCompleted;
        });
    }

    /// <summary>
    /// Opens the animal info panel and displays its data.
    /// </summary>
    void OpenAnimalInfo()
    {
        if (selectedAnimal == null) return;

        animalNameText.text = selectedAnimal.animalName;
        descriptionText.text = selectedAnimal.description;
        animalImage.texture = selectedAnimal.animalImage;

        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(true);
    }

    /// <summary>
    /// Starts the quiz for the selected animal.
    /// </summary>
    void StartQuiz()
    {
        if (selectedAnimal == null) return;
        if (quizAlreadyCompleted)
        {
            Debug.Log("Quiz already completed. Cannot retake.");
            return;
        }

        // Reset quiz stats
        quizStartTime = Time.time;
        pointsEarnedThisQuiz = 0;
        currentQuestionIndex = 0;

        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(true);

        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(true);

        LoadQuestion();
    }

    /// <summary>
    /// Loads the current question and updates answer buttons.
    /// </summary>
    void LoadQuestion()
    {
        if (selectedAnimal == null || selectedAnimal.questions == null) return;
        if (currentQuestionIndex >= selectedAnimal.questions.Count) return;

        questionText.text = selectedAnimal.questions[currentQuestionIndex];

        List<string> currentAnswers = selectedAnimal.answers[currentQuestionIndex].answers;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentAnswers[i];
        }
    }

    /// <summary>
    /// Handles answer selection, awards points if correct, and advances to next question or ends quiz.
    /// </summary>
    /// <param name="selectedIndex">Index of the selected answer button.</param>
    void OnAnswerSelected(int selectedIndex)
    {
        if (selectedAnimal == null) return;

        int correctIndex = selectedAnimal.correctAnswerIndexes[currentQuestionIndex];

        if (selectedIndex == correctIndex)
        {
            pointsEarnedThisQuiz += 10;
        }

        currentQuestionIndex++;

        if (currentQuestionIndex >= selectedAnimal.questions.Count)
            EndQuiz();
        else
            LoadQuestion();
    }

    /// <summary>
    /// Ends the quiz, hides answer buttons, and saves results to Firebase.
    /// </summary>
    void EndQuiz()
    {
        questionText.text = "Quiz Complete!";

        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(false);

        float quizDuration = Time.time - quizStartTime;

        SaveQuizResultsToFirebase(quizDuration, pointsEarnedThisQuiz);
    }

    /// <summary>
    /// Closes all UI panels and resets state.
    /// </summary>
    void CloseAllUI()
    {
        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(false);

        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(true);

        selectedAnimal = null;
        currentQuestionIndex = 0;
    }

    /// <summary>
    /// Saves the quiz results (time and points) to Firebase under the player's animal stats.
    /// </summary>
    /// <param name="duration">Duration of the quiz in seconds.</param>
    /// <param name="earnedPoints">Points earned during this quiz.</param>
    void SaveQuizResultsToFirebase(float duration, int earnedPoints)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogError("Cannot save quiz results — no user logged in!");
            return;
        }

        string uid = user.UserId;
        string animalName = selectedAnimal.animalName;

        DatabaseReference animalRef = FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("players")
            .Child(uid)
            .Child("habitats")
            .Child(currentHabitat)
            .Child(animalName);

        // Load existing data and update
        animalRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted)
            {
                Debug.LogError("Failed to load animal stats");
                return;
            }

            DataSnapshot s = task.Result;

            double oldTime = s.Child("timeTaken").Exists ? Convert.ToDouble(s.Child("timeTaken").Value) : 0;
            int oldPoints = s.Child("pointsEarned").Exists ? Convert.ToInt32(s.Child("pointsEarned").Value) : 0;

            double newTime = oldTime + duration;
            int newPoints = oldPoints + earnedPoints;

            Dictionary<string, object> update = new Dictionary<string, object>
            {
                ["timeTaken"] = newTime,
                ["pointsEarned"] = newPoints
            };

            animalRef.UpdateChildrenAsync(update);

            Debug.Log($"Saved to Firebase: [{currentHabitat}] {animalName} +{duration} sec, +{earnedPoints} pts");
        });
    }
}
