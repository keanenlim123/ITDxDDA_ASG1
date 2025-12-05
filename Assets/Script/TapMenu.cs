using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject menuPanel;
    public Button cancelButton;
    public Button button1;
    public Button button2;
    public Button button3;

    private GameObject selectedFish; // Currently selected fish

    void Start()
    {
        // Hide menu at start
        menuPanel.SetActive(false);

        // Cancel button closes the menu
        cancelButton.onClick.AddListener(CloseMenu);

        // Other buttons just print logs for now
        button1.onClick.AddListener(() => Debug.Log("Button1 clicked"));
        button2.onClick.AddListener(() => Debug.Log("Button2 clicked"));
        button3.onClick.AddListener(() => Debug.Log("Button3 clicked"));
    }

    public void panel()
    {
        menuPanel.SetActive(true);
    }

    void Update()
    {
        // Detect left click or touch
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if hit object or any child has the tag "Fish"
                var fishCollider = hit.collider;
                if (fishCollider != null)
                {
                    // If the clicked object is tagged "Fish"
                    if (fishCollider.CompareTag("Fish"))
                    {
                        selectedFish = fishCollider.gameObject;
                        ShowMenu();
                    }
                    else
                    {
                        // Check parent objects in case fish is nested
                        Transform parent = fishCollider.transform.parent;
                        while (parent != null)
                        {
                            if (parent.CompareTag("Fish"))
                            {
                                selectedFish = parent.gameObject;
                                ShowMenu();
                                break;
                            }
                            parent = parent.parent;
                        }
                    }
                }
            }
        }
    }

    void ShowMenu()
    {
        if (selectedFish != null)
        {
            Debug.Log("Fish clicked: " + selectedFish.name);
            menuPanel.SetActive(true);
        }
    }

    void CloseMenu()
    {
        menuPanel.SetActive(false);
        selectedFish = null;
    }
}
