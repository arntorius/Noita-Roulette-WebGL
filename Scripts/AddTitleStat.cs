using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AddTitleStat : MonoBehaviour
{
    public Button addButton;
    public string titleToAdd = "Default Title"; // You can set a default title in the Inspector

    void Awake()
    {
        // Ensure that the script is active
        enabled = true;

        if (addButton != null)
        {
            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(AddTitleStatEntry);
        }
        else
        {
            Debug.LogError("Add Button is not assigned!");
        }

        // Register the OnSceneLoaded method with SceneManager.sceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unregister the OnSceneLoaded method when the script is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Call the setup method when a new scene is loaded
        SetupButton();
    }

    private void SetupButton()
    {
        if (addButton != null)
        {
            addButton.onClick.RemoveAllListeners(); // Remove all previous listeners
            addButton.onClick.AddListener(AddTitleStatEntry);
        }
    }

    void AddTitleStatEntry()
    {
        Debug.Log("Adding title: " + titleToAdd);
        StatsManagerSingleton.Instance.AddRouletteStatEntry(titleToAdd, new List<int>(), null);
    }
}
