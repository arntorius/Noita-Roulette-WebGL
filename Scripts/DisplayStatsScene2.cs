
using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStatsScene2 : MonoBehaviour
{
    [SerializeField]
    private Text statsText;

    private StatsManagerSingleton statsManagerSingleton;
    private int currentHighlightedIndex = -1; // Track the currently highlighted entry index

    void Start()
    {
        statsManagerSingleton = StatsManagerSingleton.Instance;

        // Select the topmost entry on start
        currentHighlightedIndex = 0;

        DisplayStats();
    }

    void Update()
    {
        HandleArrowAndWASDKeys();
    }

    void HandleArrowAndWASDKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SelectPreviousEntry();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SelectNextEntry();
        }
    }
    void DisplayStats()
    {
        if (statsManagerSingleton != null && statsManagerSingleton.rouletteStats.Count > 0)
        {
            statsText.text = "";

            // Set fixed width for each column
            int columnWidth = 35;

            for (int i = 0; i < statsManagerSingleton.rouletteStats.Count; i++)
            {
                RouletteStatEntry entry = statsManagerSingleton.rouletteStats[i];

                // Use tabs to separate columns
                string highlight = (i == currentHighlightedIndex) ? "*\t" : "";
                string outcomeString = (entry.userOutcome == RouletteStatEntry.Outcome.Open) ? "Open" : entry.userOutcome.ToString();

                // Handle entries without results separately
                string resultText = (entry.results.Count > 0) ? string.Join(", ", entry.results) : "No Result";

                // Pad each column with spaces to ensure alignment
                string formattedText = $"{highlight}{PadRight(entry.title, columnWidth)}\t{PadRight(resultText, columnWidth)}\tOutcome {PadRight(outcomeString, columnWidth)}\tDate {PadRight(entry.date.ToShortDateString(), columnWidth)}";

                statsText.text += $"{formattedText}\n";
            }
        }
        else
        {
            statsText.text = "No stats available.";
        }
    }

    // Function to pad a string to a specified width with spaces
    string PadRight(string str, int width)
    {
        int spacesToAdd = Mathf.Max(0, width - str.Length);
        return str + new string(' ', spacesToAdd);
    }



    // Attach this method to the UI button's OnClick event
    public void DeleteSelectedEntry()
    {
        // Ensure that the StatsManagerSingleton instance is available
        if (statsManagerSingleton != null)
        {
            // Delete the currently highlighted entry
            statsManagerSingleton.DeleteRouletteStatEntry(currentHighlightedIndex);
            // Refresh the displayed stats
            DisplayStats();
        }
        else
        {
            Debug.LogError("StatsManagerSingleton instance not found.");
        }
    }
    public void SelectPreviousEntry()
    {
        currentHighlightedIndex = Mathf.Max(0, currentHighlightedIndex - 1);
        Debug.Log($"Selected previous entry: {currentHighlightedIndex}");
        DisplayStats();
    }

    public void SelectNextEntry()
    {
        currentHighlightedIndex = Mathf.Min(statsManagerSingleton.rouletteStats.Count - 1, currentHighlightedIndex + 1);
        Debug.Log($"Selected next entry: {currentHighlightedIndex}");
        DisplayStats();
    }
    public void SetOutcomeToWin()
    {
        SetOutcome(RouletteStatEntry.Outcome.Win);
    }

    public void SetOutcomeToLoss()
    {
        SetOutcome(RouletteStatEntry.Outcome.Loss);
    }

    private void SetOutcome(RouletteStatEntry.Outcome outcome)
    {
        // Set the selected outcome for the currently highlighted entry
        RouletteStatEntry entry = statsManagerSingleton.rouletteStats[currentHighlightedIndex];
        entry.userOutcome = outcome;
        Debug.Log($"Set outcome to {outcome} for entry {currentHighlightedIndex}: {entry.title}");
        DisplayStats();
    }

}
