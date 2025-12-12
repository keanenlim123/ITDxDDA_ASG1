using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class TapMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsMenuPanel;
    public GameObject animalInfoPanel;
    public GameObject quizPanel;

    [Header("Buttons")]
    public Button optionsCancelButton;
    public Button learnButton;
    public Button quizButton;
    public Button animalInfoCancelButton;
    public Button quizCancelButton;

    [Header("Animal Info UI")]
    public TMP_Text animalNameText;
    public TMP_Text descriptionText;
    public RawImage animalImage;

    [Header("Quiz UI")]
    public TMP_Text questionText;
    public Button[] answerButtons; // Size = 3

    private AnimalBehaviour selectedAnimal;
    private int currentQuestionIndex = 0;

    private bool quizAlreadyCompleted = false; // NEW

    // NEW — Tracking quiz stats
    private float quizStartTime;
    private int pointsEarnedThisQuiz = 0;

    private string currentHabitat = "Ocean"; // You can change this based on scene or object

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

    void OpenAnimalInfo()
    {
        if (selectedAnimal == null) return;

        animalNameText.text = selectedAnimal.animalName;
        descriptionText.text = selectedAnimal.description;
        animalImage.texture = selectedAnimal.animalImage;

        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(true);
    }

    void StartQuiz()
    {
        if (selectedAnimal == null) return;
        if (quizAlreadyCompleted)
        {
            Debug.Log("Quiz already completed. Cannot retake.");
            return;
        }

        // RESET EVERYTHING
        quizStartTime = 0f;
        pointsEarnedThisQuiz = 0;
        currentQuestionIndex = 0;

        quizStartTime = Time.time;

        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(true);

        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(true);

        LoadQuestion();
    }


    void LoadQuestion()
    {
        if (selectedAnimal == null) return;
        if (selectedAnimal.questions == null || selectedAnimal.questions.Count == 0) return;
        if (currentQuestionIndex >= selectedAnimal.questions.Count) return;

        questionText.text = selectedAnimal.questions[currentQuestionIndex];

        List<string> currentAnswers = selectedAnimal.answers[currentQuestionIndex].answers;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentAnswers[i];
        }
    }

    void OnAnswerSelected(int selectedIndex)
    {
        if (selectedAnimal == null) return;

        int correctIndex = selectedAnimal.correctAnswerIndexes[currentQuestionIndex];

        if (selectedIndex == correctIndex)
        {
            pointsEarnedThisQuiz += 10; // add 10 points
        }

        currentQuestionIndex++;

        if (currentQuestionIndex >= selectedAnimal.questions.Count)
        {
            EndQuiz();
        }
        else
        {
            LoadQuestion();
        }
    }

    void EndQuiz()
    {
        questionText.text = "Quiz Complete!";

        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(false);

        float quizDuration = Time.time - quizStartTime;

        SaveQuizResultsToFirebase(quizDuration, pointsEarnedThisQuiz);
    }

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

        // Load existing data
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

            Dictionary<string, object> update = new Dictionary<string, object>();
            update["timeTaken"] = newTime;
            update["pointsEarned"] = newPoints;

            animalRef.UpdateChildrenAsync(update);

            Debug.Log($"Saved to Firebase: [{currentHabitat}] {animalName} +{duration} sec, +{earnedPoints} pts");
        });
    }
}
