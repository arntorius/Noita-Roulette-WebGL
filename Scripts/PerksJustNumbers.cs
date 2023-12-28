using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerksJustNumbers : MonoBehaviour
{
    // Set these in the Inspector
    public Button rollButton;
    public Text rollText;
    public Button[] numberButtons;
    public Button stopButton;
    public Text errorMessageText;
    public InputField maxValueInput; // Add an InputField variable for the max value input
    public SpriteManager spriteManager;
    public float errorMessageOffsetX = 50f; // Default offset value, you can adjust it in the Inspector
    public Text customErrorMessageText; // Public variable to assign a different Text component for error messages



    private int maxValue;
    private int running;
    private int roll;

    IEnumerator Runner()
    {
        while (running > 0)
        {
            roll = Random.Range(0, maxValue + 1);
            rollText.text = roll.ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

   void Start()
{
    running = 0;
    maxValue = 0; // Reset maxValue to zero when the script starts

    // Set the value of each number button
    for (int i = 0; i < numberButtons.Length; i++)
    {
        int value = i + 1;
        numberButtons[i].GetComponentInChildren<Text>().text = value.ToString();
        numberButtons[i].onClick.AddListener(() => OnNumberButtonClick(value));
    }

    // Set the onClick listener for the roll button
    rollButton.onClick.AddListener(OnRollButtonClick);
    stopButton.onClick.AddListener(TaskOnClick2);
}


    void OnNumberButtonClick(int value)
    {
        // Set the max value for the random range
        maxValue = value;

        // Set the selected value to the text component
        SetValueText(value);
    }
    public void SetErrorMessage(string message)
    {
        // Set the error message and enable the Text component
        customErrorMessageText.text = message;
        customErrorMessageText.gameObject.SetActive(true);

        // Move the error message text to the right based on the errorMessageOffsetX value
        RectTransform rectTransform = customErrorMessageText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(errorMessageOffsetX, rectTransform.anchoredPosition.y);
    }
    private void AddStatEntry(string title, List<int> results)
    {
        // Use the StatsManagerSingleton to add the entry
        StatsManagerSingleton.Instance.AddRouletteStatEntry(title, results);
    }
    void OnRollButtonClick()
    {
        // Get the max value from the input field
        if (int.TryParse(maxValueInput.text, out int inputMaxValue))
        {
            // Ensure the input value is between 0 and 103
            inputMaxValue = Mathf.Clamp(inputMaxValue, 0, 103);
            maxValue = inputMaxValue;

            rollButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            if (running == 0)
            {
                running = 1;
                StartCoroutine(Runner());
            }
        }
        else
        {
            // Handle invalid input (non-integer input) and set the error message
            Debug.LogError("Invalid input. Please enter a valid number between 0 and 103.");
            SetErrorMessage("Invalid input. Please enter a valid number between 0 and 103.");
        }
    }



    public void TaskOnClick2()
    {
        running = 0;

        // Add stat entry after the roll is stopped
        AddStatEntry("Perks: Just the No.", new List<int> { roll });
    }


    // Set the selected value to the text component
    public void SetValueText(int value)
    {
        rollText.text = value.ToString();
    }
}
