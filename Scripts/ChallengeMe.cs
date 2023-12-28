using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeMe : MonoBehaviour
{
    public Text ButtonText;
    private int running;


    //texts list
    public List<string> Texts = new List<string>();

    // UI elements
    public InputField inputField;
    public Button saveChallengeButton;
    public Button myChallengesButton;
    public Button deleteChallengeButton;
    public Text challengeText;
    public RectTransform challengeTextRect;
    public Dropdown challengeDropdown;


    // Font size settings
    public int minFontSize = 12;
    public int maxFontSize = 24;

    // Key to use for saving and loading challenges in PlayerPrefs
    private const string CHALLENGE_KEY = "challenges";

    void Start()
    {
        running = 0;


        // Update font size based on available space
        UpdateChallengeTextFontSize();

        // Disable the delete challenge button initially
        deleteChallengeButton.gameObject.SetActive(false);

        // Load the challenges from PlayerPrefs
        LoadChallenges();

        // Clear the current options
        challengeDropdown.ClearOptions();

        // Add the challenges as options in the dropdown menu
        challengeDropdown.AddOptions(Texts);


    }

    IEnumerator Runner()
    {
        while (running > 0)
        {
            int textIndex = Random.Range(0, Texts.Count);
            ButtonText.text = Texts[textIndex];
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public void TaskOnClick()
    {
        int textIndex = Random.Range(0, Texts.Count);
        ButtonText.text = Texts[textIndex];
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

    // This function is called when the "Save Challenge" button is clicked

    public void SaveChallenge()
    {
        // Get the challenge text from the input field
        string challenge = inputField.text;

        // Check if the input field is not empty and does not consist only of spaces
        if (!string.IsNullOrWhiteSpace(challenge))
        {

            // Add the challenge to the list of challenges
            Texts.Add(challenge);

            // Clear the input field
            inputField.text = "";

            // Update the dropdown menu
            UpdateDropdown();

            // Save the challenges to PlayerPrefs
            SaveChallenges();
        }
    }

    // This function is called when the "My Challenges" button is clicked
    public void ShowMyChallenges()
    {

        // Show all the challenges in the list
        string challenges = "";
        for (int i = 0; i < Texts.Count; i++)
        {
            challenges += Texts[i] + "\n";
        }
        challengeText.text = challenges;
    }


    // This function is called when the "Delete Challenge" button is clicked
    public void DeleteChallenge()
    {
        // Get the index of the challenge to delete
        int index = challengeDropdown.value;

        // Remove the challenge from the list
        Texts.RemoveAt(index);

        // Update the dropdown menu
        UpdateDropdown();

        // Save the challenges to PlayerPrefs
        SaveChallenges();
    }

    // This function updates the options in the challenge dropdown menu
    private void UpdateDropdown()
    {
        // Clear the current options
        challengeDropdown.ClearOptions();

        // Calculate maximum number of characters that can fit in one line
        int maxCharactersPerLine = Mathf.FloorToInt(challengeTextRect.rect.width / challengeText.fontSize);

        // Add the challenges as options in the dropdown menu with line breaks
        foreach (string challenge in Texts)
        {
            // Insert line breaks to fit the text within available width
            string formattedChallenge = InsertLineBreaks(challenge, maxCharactersPerLine);
            challengeDropdown.options.Add(new Dropdown.OptionData(formattedChallenge));
        }

        // Set a listener for the dropdown value changed event
        challengeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private string InsertLineBreaks(string text, int maxCharactersPerLine)
    {
        // Insert line breaks to fit the text within available width
        int charactersInserted = 0;
        while (charactersInserted + maxCharactersPerLine < text.Length)
        {
            text = text.Insert(charactersInserted + maxCharactersPerLine, "\n");
            charactersInserted += maxCharactersPerLine + 1; // +1 for the inserted newline character
        }
        return text;
    }

    // Callback for Dropdown value changed event
    private void OnDropdownValueChanged(int value)
    {
        // Display the selected challenge text
        challengeText.text = Texts[value];
    }


    // This function saves the challenges to PlayerPrefs
    private void SaveChallenges()
    {
        // Convert the list of challenges to a string
        string challenges = string.Join(",", Texts.ToArray());

        // Save the challenges string to PlayerPrefs
        PlayerPrefs.SetString(CHALLENGE_KEY, challenges);
    }

    // This function loads the challenges from PlayerPrefs
    // This function loads the challenges from PlayerPrefs
    private void LoadChallenges()
    {
        // Check if the challenges key exists in PlayerPrefs
        if (!PlayerPrefs.HasKey(CHALLENGE_KEY))
        {
            // Set the default challenges
            PlayerPrefs.SetString(CHALLENGE_KEY, "Middle perk everytime,New Game +,New Game ++,New Game +++,The gods have mercy on you,+5 Shifts,No Rerolls!");
        }

        // Get the challenges string from PlayerPrefs
        string challenges = PlayerPrefs.GetString(CHALLENGE_KEY);

        // Split the challenges string into an array
        string[] challengeArray = challenges.Split(',');

        // Add the challenges to the list
        Texts.AddRange(challengeArray);
    }

    private void UpdateChallengeTextFontSize()
    {
        // Calculate maximum number of characters that can fit in the available width
        int maxCharacters = Mathf.FloorToInt(challengeTextRect.rect.width / challengeText.fontSize);

        // Calculate font size based on the maximum number of characters
        int fontSize = Mathf.Clamp(maxFontSize, minFontSize, maxFontSize);
        challengeText.fontSize = fontSize;

        // Update the text
        ShowMyChallenges();
    }

    // Update is called whenever the RectTransform changes (e.g., screen resize)
    private void Update()
    {
        UpdateChallengeTextFontSize();
    }

}