using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

        // Set answer button click listeners
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
                    optionsMenuPanel.SetActive(true);
                }
            }
        }
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

        currentQuestionIndex = 0; // 🔥 IMPORTANT: Reset before starting ANY quiz

        // Restore buttons in case previous quiz hid them
        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(true);

        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(true);

        LoadQuestion();
    }

    void LoadQuestion()
    {
        // SAFETY CHECKS
        if (selectedAnimal == null) return;
        if (selectedAnimal.questions == null || selectedAnimal.questions.Count == 0) return;
        if (currentQuestionIndex >= selectedAnimal.questions.Count) return;

        questionText.text = selectedAnimal.questions[currentQuestionIndex];

        // Get the answer list safely
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

        // Move to next question
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

        // Hide answer buttons
        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(false);
    }

    void CloseAllUI()
    {
        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        quizPanel.SetActive(false);

        // Restore buttons for next quiz
        foreach (Button btn in answerButtons)
            btn.gameObject.SetActive(true);

        selectedAnimal = null;
        currentQuestionIndex = 0; // 🔥 Prevent next animal from starting mid-quiz
    }
}
