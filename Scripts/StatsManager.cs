using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // This assumes that StatsManagerSingleton is attached to the same GameObject.
    private StatsManagerSingleton statsManagerSingleton;

    void Start()
    {
        statsManagerSingleton = FindObjectOfType<StatsManagerSingleton>();
        DisplayStats();
    }

    public void AddStatEntry(string title, List<int> results, RouletteStatEntry.Outcome userOutcome)
    {
        RouletteStatEntry newEntry = new RouletteStatEntry();
        newEntry.title = title;
        newEntry.results = results;
        newEntry.userOutcome = userOutcome;
        newEntry.date = DateTime.Now; // Set the current date and time


        // Use the singleton instance to add the entry
        statsManagerSingleton.rouletteStats.Insert(0, newEntry); // Insert at the beginning of the list

        // Display stats immediately when a new entry is added
        DisplayStats();
        Debug.Log($"AddStatEntry: Added entry - Title: {title}, Outcome: {userOutcome}");
    }


    void DisplayStats()
    {
        // No need to display stats in this script if you don't have a statsText field
        // You can leave this method empty or remove it if not needed
    }
}
