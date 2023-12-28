using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddTitleStat : MonoBehaviour
{
    public Button addButton;
    public string titleToAdd = "Default Title"; // You can set a default title in the Inspector

    void Start()
    {
        if (addButton != null)
        {
            addButton.onClick.RemoveListener(AddTitleStatEntry); // Unsubscribe previous listeners
            addButton.onClick.AddListener(AddTitleStatEntry);
        }
        else
        {
            Debug.LogError("Add Button is not assigned!");
        }
    }


    void AddTitleStatEntry()
    {
        // Add a stat entry with only a title (and possibly a date)
        StatsManagerSingleton.Instance.AddRouletteStatEntry(titleToAdd, new List<int>(), null);
    }

}
