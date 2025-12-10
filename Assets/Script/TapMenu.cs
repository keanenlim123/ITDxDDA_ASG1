using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsMenuPanel;
    public GameObject animalInfoPanel;

    [Header("Buttons")]
    public Button optionsCancelButton;
    public Button learnButton;
    public Button animalInfoCancelButton;

    [Header("Animal Info UI")]
    public TMP_Text animalNameText;
    public TMP_Text descriptionText;
    public RawImage animalImage;

    private AnimalBehaviour selectedAnimal;

    void Start()
    {
        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);

        optionsCancelButton.onClick.AddListener(CloseAllUI);
        animalInfoCancelButton.onClick.AddListener(CloseAllUI);
        learnButton.onClick.AddListener(OpenAnimalInfo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Screen clicked");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.name);

                AnimalBehaviour info = hit.collider.GetComponentInParent<AnimalBehaviour>();

                if (info != null)
                {
                    Debug.Log("Animal found: " + info.animalName);
                    selectedAnimal = info;
                    optionsMenuPanel.SetActive(true);
                }
                else
                {
                    Debug.Log("No AnimalBehaviour on hit object");
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing");
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

    void CloseAllUI()
    {
        optionsMenuPanel.SetActive(false);
        animalInfoPanel.SetActive(false);
        selectedAnimal = null;
    }
}
