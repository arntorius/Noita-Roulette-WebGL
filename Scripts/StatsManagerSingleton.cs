
using System;
using System.Collections.Generic;
using UnityEngine;
using static RouletteStatEntry;

[System.Serializable]
public class RouletteStatEntry
{
    public string title;
    public List<int> results;
    public Outcome userOutcome;
    public DateTime date;
    public string filename; // Add a new field for the filename

    public RouletteStatEntry()
    {
        // Set the default value to Open when a new instance is created
        userOutcome = Outcome.Open;
    }

    public enum Outcome
    {
        Open, // Default value is now Open
        Win,
        Loss
    }
}

public class StatsManagerSingleton : MonoBehaviour
{
    private static StatsManagerSingleton instance;
    public static StatsManagerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("StatsManagerSingleton");
                instance = go.AddComponent<StatsManagerSingleton>();

                // Ensure the GameObject persists between scenes
                DontDestroyOnLoad(go);

                // Load stats on startup
                instance.LoadStats();
            }
            else if (instance != null && instance != FindObjectOfType<StatsManagerSingleton>())
            {
                Destroy(instance.gameObject);
                instance = null;
                return Instance;
            }

            return instance;
        }
    }



    public List<RouletteStatEntry> rouletteStats = new List<RouletteStatEntry>();

    // Add or update a roulette stat entry
    public void AddRouletteStatEntry(string title, List<int> results, string filename = null)
    {
        // Check if an entry with the same title already exists
        RouletteStatEntry existingEntry = rouletteStats.Find(entry => entry.title == title);

        // Add a new entry at the beginning of the list
        rouletteStats.Insert(0, new RouletteStatEntry
        {
            title = title,
            results = results,
            userOutcome = Outcome.Open, // Set the default outcome to Open
            date = System.DateTime.Now,
            filename = filename // Set the filename
        });
    }

    public void AddBountyHuntResult(int numCreatures)
    {
        string title = "The Bounty Hunt";
        List<int> results = new List<int> { numCreatures };

        // Add the results to the StatsManager
        AddRouletteStatEntry(title, results);
    }
    public void AddResult(string title, int result)
    {
        // Check if an entry with the same title already exists
        RouletteStatEntry existingEntry = rouletteStats.Find(entry => entry.title == title);

        if (existingEntry != null)
        {
            // Add the result to the existing entry
            existingEntry.results = new List<int> { result };
            SaveStats();
        }
        else
        {
            // Add a new entry with the result
            AddRouletteStatEntry(title, new List<int> { result });
        }
    }

    // Delete a roulette stat entry at the specified index
    public void DeleteRouletteStatEntry(int index)
    {
        if (index >= 0 && index < rouletteStats.Count)
        {
            rouletteStats.RemoveAt(index);
            SaveStats(); // Save the updated stats after deletion
        }
        else
        {
            Debug.LogWarning($"Invalid index for deletion: {index}");
        }
    }

    // Save the stats to PlayerPrefs
    public void SaveStats()
    {
        string statsJson = JsonUtility.ToJson(rouletteStats);
        PlayerPrefs.SetString("RouletteStats", statsJson);
        PlayerPrefs.Save();
    }

    // Load the stats from PlayerPrefs
    private void LoadStats()
    {
        if (PlayerPrefs.HasKey("RouletteStats"))
        {
            string statsJson = PlayerPrefs.GetString("RouletteStats");
            rouletteStats = JsonUtility.FromJson<List<RouletteStatEntry>>(statsJson);
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Load stats on startup
            LoadStats();
            DontDestroyOnLoad(gameObject);  // Keep the GameObject persistent between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
