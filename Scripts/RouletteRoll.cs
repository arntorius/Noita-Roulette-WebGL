using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class RouletteRoll : MonoBehaviour
{
    public GameObject[] TextBoxes;
    public Button RollButton;
    public StatsManager statsManager; // Reference to the StatsManager

    private int[] theNumbers;
    private int running;

    void Start()
    {
        running = 0;
        theNumbers = new int[TextBoxes.Length];

        // Initialize statsManager reference
        statsManager = FindObjectOfType<StatsManager>();

        if (statsManager == null)
        {
            Debug.LogError("StatsManager not found in the scene. Make sure it is present.");
        }
    }

    IEnumerator Runner()
    {
        while (running > 0)
        {
            for (int i = 0; i < TextBoxes.Length; i++)
            {
                theNumbers[i] = Random.Range(0, 12);
                TextBoxes[i].GetComponent<Text>().text = theNumbers[i].ToString();
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        // When the script finishes running, save the stats to the StatsManager
        string title = "Standard Roulette 0-11";
        SaveStats(title, theNumbers);
    }

    void SaveStats(string title, int[] results)
    {
        // Convert the int array to a List<int>
        List<int> resultList = new List<int>(results);

        // Add the results to the StatsManager with the default Outcome.Open
        statsManager.AddStatEntry(title, resultList, RouletteStatEntry.Outcome.Open);
        Debug.Log("SaveStats called");
    }

    public void TaskOnClick()
    {
        if (running == 0)
        {
            running = 1;
            StartCoroutine(Runner());
        }
    }

    public void TaskOnClick2()
    {
        running = 0;
    }
}
