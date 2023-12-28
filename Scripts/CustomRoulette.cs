using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static RouletteStatEntry;

public class CustomRoulette : MonoBehaviour
{
    // Set these in the Inspector
    public Button overallRollButton;
    public Button overallStopButton;
    public Text[] valueTexts;
    public Button[] numberButtonsCategory1;
    public Button[] numberButtonsCategory2;
    public Button[] numberButtonsCategory3;
    public Text[] rollTexts;

    private int[] maxValues;
    private int[] running;
    private int[] rolls;

    // Reference to the StatsManagerSingleton
    private StatsManagerSingleton statsManager;

    private bool resultsAdded;

    IEnumerator Runner(int index)
    {
        Debug.Log($"Runner coroutine started for index {index}");

        resultsAdded = false;  // Reset the flag for a new run

        while (running[index] > 0)
        {
            rolls[index] = UnityEngine.Random.Range(0, maxValues[index] + 1);
            rollTexts[index].text = rolls[index].ToString();

            yield return new WaitForSecondsRealtime(0.15f);
        }

        // Add the results to the StatsManagerSingleton only if not added before
        if (!resultsAdded)
        {
            string title = $"Custom Roulette";
            List<int> results = rolls.Take(rolls[index] + 1).ToList();
            statsManager.AddRouletteStatEntry(title, results);
            Debug.Log($"Results added to StatsManagerSingleton: {string.Join(", ", results)}");

            resultsAdded = true;  // Mark results as added
        }
    }

    // Modified method name to avoid conflicts
    void OnOverallRollButtonClickCustom(int index)
    {
        overallRollButton.gameObject.SetActive(false);
        overallStopButton.gameObject.SetActive(true);

        rolls[index] = UnityEngine.Random.Range(0, maxValues[index] + 1);
        rollTexts[index].text = rolls[index].ToString();

        if (running[index] == 0)
        {
            running[index] = 1;
            resultsAdded = false;  // Reset resultsAdded flag for a new run
            StartCoroutine(Runner(index));
        }
    }

    void Start()
    {
        int numScripts = 3;

        maxValues = new int[numScripts];
        running = new int[numScripts];
        rolls = new int[numScripts];

        // Get the reference to the StatsManagerSingleton
        statsManager = StatsManagerSingleton.Instance;

        for (int i = 0; i < numScripts; i++)
        {
            InitializeScript(i);
        }

        overallRollButton.onClick.AddListener(() => OnOverallRollButtonClick(0));
        overallStopButton.onClick.AddListener(() => TaskOnClick2(0));
    }

    void InitializeScript(int index)
    {
        running[index] = 0;
        maxValues[index] = 20; // default max value

        // Determine which category the button belongs to
        Button[] numberButtons = null;
        switch (index)
        {
            case 0:
                numberButtons = numberButtonsCategory1;
                break;
            case 1:
                numberButtons = numberButtonsCategory2;
                break;
            case 2:
                numberButtons = numberButtonsCategory3;
                break;
        }

        if (numberButtons != null)
        {
            for (int i = 0; i < numberButtons.Length; i++)
            {
                int value = i + 1;
                numberButtons[i].GetComponentInChildren<Text>().text = value.ToString();
                int currentIndex = index; // To avoid closure-related issues
                numberButtons[i].onClick.AddListener(() => OnNumberButtonClick(currentIndex, value));
            }
        }

        overallRollButton.onClick.AddListener(() => OnOverallRollButtonClick(index));
        overallStopButton.onClick.AddListener(() => TaskOnClick2(index));
    }

    void OnNumberButtonClick(int index, int value)
    {
        maxValues[index] = value;
        SetValueText(index, value);
    }

    void OnOverallRollButtonClick(int index)
    {
        overallRollButton.gameObject.SetActive(false);
        overallStopButton.gameObject.SetActive(true);

        rolls[index] = UnityEngine.Random.Range(0, maxValues[index] + 1);
        rollTexts[index].text = rolls[index].ToString();

        if (running[index] == 0)
        {
            running[index] = 1;
            StartCoroutine(Runner(index));
        }
    }

    void TaskOnClick2(int index)
    {
        running[index] = 0;
    }

    void SetValueText(int index, int value)
    {
        valueTexts[index].text = value.ToString();
    }
}
