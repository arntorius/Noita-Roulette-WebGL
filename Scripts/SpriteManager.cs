using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public GameObject[] spritePrefabs;
    public Transform container;
    public InputField inputField;
    public Button giveSetButton;
    public Button resetButton;
    public Button randomSetButton; // Reference to the button for generating random numbers
    public Button printButton; // Reference to the UI button for exporting sprite filenames
    public Text errorMessageText; // Reference to the UI Text element for displaying error messages
    public Text rolledNumberText; // Reference to the UI Text element for displaying the rolled number
    public InputField maxValueInput;
    public float unmuteDuration = 0.2f; // Duration in seconds to unmute the audio
    public float rollDuration = 1.0f; // Roll duration in seconds
    public float displayDuration = 2f; // Duration in seconds to display the rolled number
    public AudioClip rollingSound; // Reference to the rolling sound effect
    public AudioClip endingSound; // Reference to the ending sound effect
    public float rollSoundSpeed = 1.0f; // Speed of the rolling sound effect
    // private bool isRolling = false;
    private Coroutine rollingCoroutine;
    private int totalSprites;
    private GameObject[] shuffledSprites;
    public float horizontalSpacing = 10f;
    public float verticalSpacing = 10f;
    public int spritesPerRow = 5;
    public int numberOfRows = 2;
    private AudioSource rollingAudioSource;
    private List<GameObject> instantiatedSprites = new List<GameObject>(); // List to store instantiated sprites
    private int rolledNumber;




    public bool CanAddNewSprite()
    {
        // Implement your logic to check if adding a new sprite is allowed, for example, based on game state or conditions
        return true; // Return true if adding a new sprite is allowed, otherwise return false
    }
    public static SpriteManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        maxValueInput.text = "10";
        giveSetButton.onClick.AddListener(StartRolling);
        resetButton.onClick.AddListener(ResetSprites);
        randomSetButton.onClick.AddListener(GenerateRandomSprites);
        printButton.onClick.AddListener(ExportSpriteFilenames);

    }
    private void StartRolling()
    {
        // Clear existing error message
        errorMessageText.text = "";

        // Clear existing error message
        HideErrorMessage();

        // Mute audio first
        MuteAllAudio();

        // Clear existing sprites
        ClearSprites();

        if (int.TryParse(inputField.text, out int inputNumber) && inputNumber > 0 && inputNumber <= 103)
        {
            // Calculate total number of sprites to display
            totalSprites = Mathf.Min(inputNumber, spritesPerRow * numberOfRows);

            // Create a copy of the spritePrefabs array to shuffle temporarily
            GameObject[] tempSprites = spritePrefabs.Clone() as GameObject[];

            // Store the rolled number
            rolledNumber = Random.Range(0, inputNumber + 1);
            Debug.Log("Rolled Number: " + rolledNumber);

            // Shuffle the temporary array
            for (int i = 0; i < tempSprites.Length; i++)
            {
                int randomIndex = Random.Range(i, tempSprites.Length);
                GameObject temp = tempSprites[i];
                tempSprites[i] = tempSprites[randomIndex];
                tempSprites[randomIndex] = temp;
            }

            // Initialize the shuffledSprites array and instantiate the GameObjects
            shuffledSprites = new GameObject[totalSprites];
            for (int i = 0; i < totalSprites; i++)
            {
                // Instantiate the shuffled sprite and store the reference
                GameObject spriteObject = Instantiate(tempSprites[i], container.transform);
                RectTransform rectTransform = spriteObject.GetComponent<RectTransform>();

                // Add a 2D collider to handle mouse clicks
                BoxCollider2D collider = spriteObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

                // Get or add AudioSource component to the sprite object
                AudioSource audioSource = spriteObject.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = spriteObject.AddComponent<AudioSource>();
                }

                // Get or add SpriteClickHandler component to the sprite object
                SpriteClickHandler clickHandler = spriteObject.GetComponent<SpriteClickHandler>();
                if (clickHandler == null)
                {
                    clickHandler = spriteObject.AddComponent<SpriteClickHandler>();
                }

                // Initialize SpriteClickHandler with SpriteManager, index, audioSource
                clickHandler.Initialize(this, i, audioSource);

                // ... (calculate position and set anchored position)
            }

            // Start the rolling coroutine with the specified duration
            rollingCoroutine = StartCoroutine(RollSprites(totalSprites));

            // Add stat entry for the initiated roll
            AddStatEntry("Perks: Give set No.", new List<int> { inputNumber });
        }
        else
        {
            // Show error message if input is not valid
            SetErrorMessage("Invalid input. Please enter a number between 1 and 103.");
        }
    }

    private void GenerateRandomSprites()
    {
        // Clear existing error message
        errorMessageText.text = "";

        if (int.TryParse(inputField.text, out int inputNumber) && inputNumber > 0 && inputNumber <= spritePrefabs.Length)
        {
            // Generate a random number between 0 and inputNumber
            int rolledNumber = Random.Range(0, inputNumber + 1);

            // Delay the sprite rollout and wait for the number rollout to finish
            StartCoroutine(DelayedRollSprites(rolledNumber, inputNumber));

            // Add stat entry for the initiated random roll
            AddStatEntry("Perks: Gamble", new List<int> { rolledNumber });
        }
        else
        {
            // Show error message if input is not valid
            SetErrorMessage("Invalid input. Please enter a number between 1 and " + spritePrefabs.Length + ".");
        }
    }
    private void AddStatEntry(string title, List<int> results)
    {
        // Use the StatsManagerSingleton to add the entry
        StatsManagerSingleton.Instance.AddRouletteStatEntry(title, results);
    }
    public void SetErrorMessage(string message)
    {
        // Set the error message and enable the Text component
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }

    public void HideErrorMessage()
    {
        // Disable the Text component to hide the error message
        errorMessageText.gameObject.SetActive(false);
    }
    private void DisplaySprites()
    {
        // Clear existing sprites
        ClearSprites();

        if (int.TryParse(inputField.text, out int inputNumber) && inputNumber > 0)
        {
            // Calculate total number of sprites to display
            totalSprites = Mathf.Min(inputNumber, spritesPerRow * numberOfRows);

            // Create a copy of the spritePrefabs array to shuffle temporarily
            GameObject[] tempSprites = spritePrefabs.Clone() as GameObject[];

            // Shuffle the temporary array
            for (int i = 0; i < tempSprites.Length; i++)
            {
                int randomIndex = Random.Range(i, tempSprites.Length);
                GameObject temp = tempSprites[i];
                tempSprites[i] = tempSprites[randomIndex];
                tempSprites[randomIndex] = temp;
            }

            // Initialize the shuffledSprites array and instantiate the GameObjects
            shuffledSprites = new GameObject[totalSprites];
            for (int i = 0; i < totalSprites; i++)
            {
                // Instantiate the shuffled sprite and store the reference
                shuffledSprites[i] = Instantiate(tempSprites[i], container.transform);
                GameObject spriteObject = Instantiate(shuffledSprites[i], container);
                RectTransform rectTransform = spriteObject.GetComponent<RectTransform>();

                // Add a 2D collider to handle mouse clicks
                BoxCollider2D collider = spriteObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

                // Attach a custom script to handle sprite swapping logic
                SpriteClickHandler clickHandler = spriteObject.AddComponent<SpriteClickHandler>();
                AudioSource audioSource = spriteObject.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = spriteObject.AddComponent<AudioSource>();
                }
                clickHandler.Initialize(this, i, audioSource);

                // ... (calculate position and set anchored position)
            }
            // Start the roll coroutine with the specified duration
            StartCoroutine(RollSprites(totalSprites));
        }
        else
        {
            Debug.LogError("Invalid input. Please enter a valid number.");
        }
    }
    private IEnumerator DisplayRolledNumberForDuration(int rolledNumber)
    {
        // Set the rolled number text field to be visible
        rolledNumberText.gameObject.SetActive(true);

        int currentNumber = 1;
        float rollDuration = 0.01f; // Duration for each number change in seconds

        // Simulate rolling effect
        while (currentNumber <= rolledNumber)
        {
            rolledNumberText.text = currentNumber.ToString();

            // Play the rolling sound effect for each number
            if (rollingSound != null)
            {
                AudioSource.PlayClipAtPoint(rollingSound, Camera.main.transform.position);
            }

            // Wait for a short duration before showing the next number
            yield return new WaitForSeconds(rollDuration);

            currentNumber++;
        }

        // Play the ending sound effect
        if (endingSound != null)
        {
            AudioSource.PlayClipAtPoint(endingSound, Camera.main.transform.position);
        }

        // Wait for the specified duration after the rolling effect is complete
        yield return new WaitForSeconds(displayDuration - (rollDuration * (rolledNumber - 1)));

        // Hide the rolled number text field after the duration
        rolledNumberText.gameObject.SetActive(false);

        // Add the rolled number to StatsManagerSingleton without specifying the outcome
        StatsManagerSingleton.Instance.AddResult("Perks:Set No.", rolledNumber);
    }
    private void ClearSprites()
    {
        // Clear existing sprites
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    private void MuteAllAudio()
    {
        // Mute audio by setting the volume to 0
        foreach (var prefab in spritePrefabs)
        {
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = 0f;
            }
        }
    }
    private IEnumerator UnmuteAllAudioWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Unmute audio by setting the volume back to 1
        foreach (var prefab in spritePrefabs)
        {
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = 1f;
            }
        }
    }
    public void UnmuteAllAudio()
    {
        // Unmute audio by setting the volume back to 1
        foreach (var prefab in spritePrefabs)
        {
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = 1f;
            }
        }
    }
    private IEnumerator DelayedRollSprites(int rolledNumber, int inputNumber)
    {
        // Update the totalSprites variable with the rolled number
        totalSprites = rolledNumber;

        // Roll the number first
        StartCoroutine(DisplayRolledNumberForDuration(rolledNumber));

        // Wait for the number rollout to finish
        yield return new WaitForSeconds(displayDuration);

        // Clear existing sprites
        ClearSprites();

        // Roll sprites after the number rollout is complete
        StartCoroutine(RollSprites(totalSprites));
    }
    private IEnumerator RollSprites(int totalSprites)
    {
        //isRolling = true;

        // Create a copy of the spritePrefabs array to shuffle temporarily
        GameObject[] shuffledSprites = spritePrefabs.Clone() as GameObject[];

        // Play the rolling sound continuously in the background at the specified speed
        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.clip = rollingSound;
        rollingAudioSource.loop = true;
        rollingAudioSource.pitch = rollSoundSpeed; // Set the pitch to control the speed
        rollingAudioSource.Play();

        // Shuffle the temporary array for a short duration
        float rollDuration = 0.1f; // Duration for the shuffle effect in seconds
        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            // Shuffle the temporary array
            for (int i = 0; i < shuffledSprites.Length; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Length);
                GameObject temp = shuffledSprites[i];
                shuffledSprites[i] = shuffledSprites[randomIndex];
                shuffledSprites[randomIndex] = temp;
            }

            // Wait for a short interval before shuffling again
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Clear existing sprites and audio sources
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Display the final shuffled sprites with audio sources
        for (int i = 0; i < totalSprites; i++)
        {
            GameObject spriteObject = Instantiate(shuffledSprites[i], container);
            RectTransform rectTransform = spriteObject.GetComponent<RectTransform>();

            // Calculate position based on row and column with spacing
            float xPos = (i % spritesPerRow) * (rectTransform.rect.width + horizontalSpacing);
            float yPos = -(i / spritesPerRow) * (rectTransform.rect.height + verticalSpacing);
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);

            // Wait for the specified duration before displaying the next sprite
            yield return new WaitForSeconds(rollDuration);
        }

        // Stop the rolling sound when the rolling animation is complete
        rollingAudioSource.Stop();

        // Clean up the temporary audio source
        Destroy(rollingAudioSource);

        // Optionally, you can play the ending sound effect here if desired.
    }

    private void ExportSpriteFilenames()
    {
        List<string> spriteFilenames = new List<string>();

        // Extract filenames from sprites inside the container
        foreach (Transform child in container)
        {
            GameObject instantiatedSprite = child.gameObject;
            if (instantiatedSprite != null)
            {
                SpriteRenderer spriteRenderer = instantiatedSprite.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    spriteFilenames.Add(spriteRenderer.sprite.name);
                }
            }
        }
    }

    private void ResetSprites()
    {
        //isRolling = false; // Stop the rolling coroutine

        // Clear existing sprites
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Stop the rolling sound
        if (rollingAudioSource != null && rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Stop();
            Destroy(rollingAudioSource); // Destroy the AudioSource component
        }

        // Clear the input field
        // inputField.text = "";
    }


}
