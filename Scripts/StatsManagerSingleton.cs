using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class RouletteStatEntry
{
    public string title;
    public List<int> results;
    public Outcome userOutcome;
    public DateTime date; // Remove [System.NonSerialized]
    public string filename;

    public RouletteStatEntry()
    {
        userOutcome = Outcome.Open;
        date = DateTime.Now;
    }

    public enum Outcome
    {
        Open,
        Win,
        Loss
    }
}



public class StatsManagerSingleton : MonoBehaviour
{
    public UnityEvent OnStatsUpdated = new UnityEvent();

    private static StatsManagerSingleton instance;
    private const string PlayerPrefsKey = "RouletteStats";

    public List<RouletteStatEntry> rouletteStats = new List<RouletteStatEntry>();


    public static StatsManagerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("StatsManagerSingleton");
                instance = go.AddComponent<StatsManagerSingleton>();
                DontDestroyOnLoad(go);
                instance.LoadStatsFromPlayerPrefs();
            }
            return instance;
        }
    }
    void Awake()
    {
        Debug.Log("StatsManagerSingleton Awake method called.");

        if (instance == null)
        {
            instance = this;
            LoadStatsFromPlayerPrefs();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadStatsFromPlayerPrefs();
    }

    public void AddRouletteStatEntry(string title, List<int> results, string filename = null)
    {
        RouletteStatEntry existingEntry = rouletteStats.Find(entry => entry.title == title);

        RouletteStatEntry newEntry = new RouletteStatEntry
        {
            title = title,
            results = results,
            date = DateTime.Now,
            filename = filename
        };

        // Set userOutcome to the last saved state or Open if it's a new entry
        newEntry.userOutcome = existingEntry != null ? existingEntry.userOutcome : RouletteStatEntry.Outcome.Open;

        rouletteStats.Insert(0, newEntry);

        SaveStatsToPlayerPrefs();
    }


    // public void AddBountyHuntResult(int numCreatures)
    // {
       // string title = "The Bounty Hunt";
       // List<int> results = new List<int> { numCreatures };
       // AddRouletteStatEntry(title, results);
    // }

   // public void AddResult(string title, int result)
    //{
      //  RouletteStatEntry existingEntry = rouletteStats.Find(entry => entry.title == title);

        //if (existingEntry != null)
        //{
          //  existingEntry.results = new List<int> { result };
            
            //SaveStatsToPlayerPrefs();
        //}
        //else
        //{
           // AddRouletteStatEntry(title, new List<int> { result });
        //}
    //}

    public void DeleteRouletteStatEntry(int index)
    {
        if (index >= 0 && index < rouletteStats.Count)
        {
            rouletteStats.RemoveAt(index);
            
            SaveStatsToPlayerPrefs();
        }
        else
        {
            Debug.LogWarning($"Invalid index for deletion: {index}");
        }
    }

    public void SaveStatsToPlayerPrefs()
    {
        string statsString = ConvertStatsListToString(rouletteStats);
        PlayerPrefs.SetString(PlayerPrefsKey, statsString);
        PlayerPrefs.Save();

        Debug.Log("Stats saved to PlayerPrefs:");
        Debug.Log(statsString);

        OnStatsUpdated.Invoke();
    }
    private void LoadStatsFromPlayerPrefs()
    {
        Debug.Log("Loading stats from PlayerPrefs.");

        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string statsJson = PlayerPrefs.GetString(PlayerPrefsKey);
            Debug.Log($"Loaded stats JSON: {statsJson}");

            if (!string.IsNullOrEmpty(statsJson) && statsJson != "{}")
            {
                List<RouletteStatEntry> loadedStats = ConvertStatsStringToList(statsJson);

                // Update userOutcome based on the last saved state
                foreach (var entry in loadedStats)
                {
                    RouletteStatEntry lastSavedState = rouletteStats.Find(x => x.title == entry.title);
                    if (lastSavedState != null)
                    {
                        entry.userOutcome = lastSavedState.userOutcome;
                    }
                }

                rouletteStats = loadedStats;
                Debug.Log("Stats loaded successfully.");
            }
            else
            {
                Debug.Log("Loaded stats JSON is empty or invalid. Initializing with default values.");
                rouletteStats = new List<RouletteStatEntry>();
            }

            OnStatsUpdated.Invoke();
        }
        else
        {
            Debug.Log("No stats found in PlayerPrefs. Initializing with default values.");
            rouletteStats = new List<RouletteStatEntry>();
        }
    }


    private List<RouletteStatEntry> ConvertStatsStringToList(string statsString)
    {
        // Remove the curly braces from the JSON string to get an array of objects
        statsString = statsString.TrimStart('{').TrimEnd('}');

        // Split the string into individual stat entries
        string[] statsArray = statsString.Split(new[] { "},{" }, StringSplitOptions.None);

        List<RouletteStatEntry> statsList = new List<RouletteStatEntry>();

        foreach (string statEntry in statsArray)
        {
            // Add the curly braces back to each entry to form a valid JSON object
            string statJson = "{" + statEntry + "}";
            RouletteStatEntry stat = JsonUtility.FromJson<RouletteStatEntry>(statJson);
            statsList.Add(stat);
        }

        return statsList;
    }


    private string ConvertStatsListToString(List<RouletteStatEntry> statsList)
    {
        List<string> statsStringList = new List<string>();

        foreach (RouletteStatEntry stat in statsList)
        {
            string statString = JsonUtility.ToJson(stat);
            statsStringList.Add(statString);
        }

        string statsString = string.Join(",", statsStringList.ToArray());
        return statsString;
    }


}
