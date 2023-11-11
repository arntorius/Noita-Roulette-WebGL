using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Text;

[System.Serializable]

public class FolderData2
{
    public string folderName;
    public string folderPath; // Add folder path property
    public int weight;
    public int folderIndex; // Add folder index property
}

public class CreatureManager : MonoBehaviour
{
    public Button spawnButton;
    public Button rerollButton;
    public Text rerollButtonText; // Reference to the Text component of the reroll button
    public Button destroyButton;
    public Button printButton;
    public Transform container;
    public AudioClip audioClip;
    public Text resultText;
    public float spacing = 1f;
    public List<FolderData2> folders;
    private int numSprites; // Variable to store the number of sprites for reroll
    private int rerollCount = 3; // Number of allowed rerolls
    private int currentReroll = 0; // Counter for the current reroll attempts
    // List to keep track of spawned additional sprites
    private List<AdditionalSpriteMarkerForCreatureManager> spawnedAdditionalSprites = new List<AdditionalSpriteMarkerForCreatureManager>();



    [SerializeField]
    private GameObject spritePrefab;

    [SerializeField]
    private GameObject checkMarkPrefab;

    [SerializeField]
    private Text additionalSpritesTextField; // Reference to the Text component in the Unity Inspector

    [Header("Completion Effects")]
    [SerializeField]
    private AudioClip completionAudioClip; // Assign this in the Unity Inspector

    [SerializeField]
    private Text completionText; // Assign this in the Unity Inspector


    [SerializeField]
    private int rows = 3; // Number of rows for the formatted text

    private int columns = 3; // Number of columns for the formatted text

    private List<string> additionalSpriteNames = new List<string>();

    public List<string> AdditionalSpriteNames
    {
        get { return additionalSpriteNames; }
    }
    private void Start()
    {
        spawnButton.onClick.AddListener(OnSpawnButtonPress);
        destroyButton.onClick.AddListener(OnDestroyButtonPress);
        rerollButton.onClick.AddListener(OnRerollButtonPress);
        printButton.onClick.AddListener(OnPrintButtonPress);
        rerollButton.interactable = true; // Enable the reroll button at the start
        UpdateRerollButtonText(); // Update the reroll button text at the start
    }
    
    private void OnResetButtonClick()
    {
        UpdateRerollButtonText(); // Update the reroll button text at the start
    }
    private void OnDestroyButtonPress()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        resultText.text = "";
        rerollButton.interactable = true; // Enable the reroll button after destroying existing sprites
        UpdateRerollButtonText(); // Update the reroll button text after destroying existing sprites

        // Reset the rerollCount count to 3 when reroll button is pressed
        rerollCount = 3; // Reset the counter here
        Debug.Log("rerollCount Count Reset to: " + rerollCount);
    }
    private void OnRerollButtonPress()
    {
        if (currentReroll < rerollCount)
        {
            // Destroy all existing sprites before rerolling
            DestroyAllSprites();

            // Increment the number of sprites for the next reroll
            numSprites += 1;

            // Update the columns variable based on the result of Random.Range
            columns = Random.Range(1, numSprites + 2); // +1 on every reroll button press

            // Reroll with the updated number of sprites and columns
            StartCoroutine(PlaceSpritePrefabs(numSprites));
            resultText.text = "Number of Creatures: " + (numSprites * 3).ToString();

            currentReroll++; // Increment the reroll counter
            if (currentReroll >= rerollCount)
            {
                rerollButton.interactable = true; // Disable the reroll button after reaching the allowed rerolls
            }

            UpdateRerollButtonText(); // Update the reroll button text after each reroll attempt
        }
    }

    public void RemoveAdditionalSprites()
    {
        AdditionalSpriteMarker[] additionalSprites = container.GetComponentsInChildren<AdditionalSpriteMarker>();
        foreach (AdditionalSpriteMarker additionalSprite in additionalSprites)
        {
            Destroy(additionalSprite.gameObject);
            // Decrement the count of spritePrefab2 instances
            
        }
    }

    public void DestroyAllSprites()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        resultText.text = "";
        rerollButton.interactable = true; // Enable the reroll button after destroying existing sprites
        UpdateRerollButtonText(); // Update the reroll button text after destroying existing sprites
    }

    private void UpdateRerollButtonText()
    {
        int remainingRerolls = rerollCount - currentReroll;
        if (remainingRerolls > 1)
        {
            rerollButtonText.text = remainingRerolls + " Re-Rolls Left";
        }
        else if (remainingRerolls == 1)
        {
            rerollButtonText.text = " Re-Rolls Left";
        }
        else
        {
            rerollButtonText.text = "Good Luck";
        }
    }
    private void OnSpawnButtonPress()
    {
        numSprites = Random.Range(3, 16);
        StartCoroutine(PlaceSpritePrefabs(numSprites));
        resultText.text = "Number of Creatures: " + (numSprites * 3).ToString();
        currentReroll = 0; // Reset the reroll counter when spawning new sprites
        rerollButton.interactable = true; // Enable the reroll button when spawning new sprites
        UpdateRerollButtonText(); // Update the reroll button text at the start
    }

    private void OnPrintButtonPress()
    {
        StartCoroutine(DelayedPrint());
    }
    private IEnumerator DelayedPrint()
    {
        // Wait for a short duration to ensure all sprites are generated
        yield return new WaitForSeconds(0.1f);

        // Generate the list of additional sprite names
        GenerateAdditionalSpriteNames();

        // Display the sprite names on screen
        DisplaySpriteNamesOnScreen();

        // Wait for a few frames before logging the number of additional sprites and their names
        yield return null;

        Debug.Log("Number of Additional Sprites: " + additionalSpriteNames.Count);
        foreach (string spriteName in additionalSpriteNames)
        {
            Debug.Log("Additional Sprite Name: " + spriteName);
        }
    }
    private void GenerateAdditionalSpriteNames()
    {
        Debug.Log("Generating Additional Sprite Names");

        additionalSpriteNames.Clear(); // Clear the list before generating new names

        // Find all objects with the AdditionalSpriteMarker script in the scene
        AdditionalSpriteMarkerForCreatureManager[] additionalSprites = FindObjectsOfType<AdditionalSpriteMarkerForCreatureManager>();

        foreach (AdditionalSpriteMarkerForCreatureManager additionalSprite in additionalSprites)
        {
            // Log the GameObject name for debugging
            Debug.Log("Found Additional Sprite in GameObject: " + additionalSprite.gameObject.name);

            SpriteRenderer spriteRenderer = additionalSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                string spriteName = spriteRenderer.sprite.name;
                additionalSpriteNames.Add(spriteName);

                // Log the sprite name for debugging
                Debug.Log("Added Sprite Name: " + spriteName);
            }
            else
            {
                // Log a warning if a sprite renderer or sprite is not found
                Debug.LogWarning("Sprite Renderer or Sprite not found in AdditionalSprite: " + additionalSprite.name);
            }
        }
    }
    private void DisplaySpriteNamesOnScreen()
    {
        // Display the sprite names in the assigned TextField in the Inspector with formatting options
        if (additionalSpritesTextField != null)
        {
            additionalSpriteNames.Reverse(); // Reverse the order of the list

            string spriteNamesText = "";

            int count = 0;

            for (int i = 0; i < additionalSpriteNames.Count; i++)
            {
                count++;
                spriteNamesText += count + ". " + additionalSpriteNames[i] + "\t"; // Add a number for each entry

                if (count % numSprites == 0)
                {
                    spriteNamesText += "\n"; // Start a new row after 'numSprites' number of items
                }

                if (count >= numSprites * 3)
                {
                    break; // Limit the displayed items to numSprites * 3
                }
            }

            // Set the text of the text field
            additionalSpritesTextField.text = spriteNamesText;

            // Adjust the size of the text field based on its content
            LayoutRebuilder.ForceRebuildLayoutImmediate(additionalSpritesTextField.GetComponent<RectTransform>());
        }
    }

    private HashSet<string> usedSpritesAcrossLines = new HashSet<string>(); // Track used sprites across all lines

    private IEnumerator PlaceSpritePrefabs(int numSprites)
    {
        float totalWidth = 0f;
        List<string> allFolderPaths = new List<string>();
        int totalSpritesSpawned = 0;

        foreach (var folder in folders)
        {
            for (int i = 0; i < folder.weight; i++)
            {
                allFolderPaths.Add(folder.folderPath);
            }
        }

        // Shuffle the list of folder paths
        ShuffleList(allFolderPaths);
        Debug.Log("Shuffled Folder Paths: " + string.Join(", ", allFolderPaths));

        // Calculate the y-offset for each line
        float yOffset = -0.03f;

        // Instantiate three lines of sprites simultaneously
        List<string> usedSpritesAcrossLines = new List<string>(); // Track used sprites across all lines

        for (int lineIndex = 0; lineIndex < 3; lineIndex++)
        {
            totalWidth = 0f; // Reset totalWidth for each new line
            List<string> usedSpritesInLine = new List<string>(); // Track used sprites for this line

            for (int i = 0; i < numSprites; i++)
            {
                GameObject spriteObject = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity, container);
                SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>(); // Add this line
                float spriteWidth = spriteRenderer.bounds.size.x; // Change spritePrefab to spriteRenderer

                // Set the sprite of spriteObject to the spritePrefab
                spriteRenderer.sprite = spritePrefab.GetComponent<SpriteRenderer>().sprite;

                // Instantiate the additional sprite object
                GameObject additionalSpriteObject = new GameObject("AdditionalSprite");
                AdditionalSpriteMarkerForCreatureManager additionalSpriteMarker = additionalSpriteObject.AddComponent<AdditionalSpriteMarkerForCreatureManager>();

                totalWidth += spriteWidth + spacing;

                float startX = -totalWidth / 2f;

                for (int j = 0; j < i; j++)
                {
                    Transform existingSprite = container.GetChild(j + lineIndex * numSprites);
                    float offset = startX + j * spriteWidth + j * spacing;
                    Vector3 newPosition = new Vector3(offset, lineIndex * yOffset, 0);
                    existingSprite.localPosition = newPosition;
                }

                // Use the modified positioning logic here
                float spriteX = startX + i * spriteWidth + i * spacing;
                spriteObject.transform.localPosition = new Vector3(spriteX, lineIndex * yOffset, 0);

                AudioSource.PlayClipAtPoint(audioClip, spriteObject.transform.position);

                yield return new WaitForSeconds(0.1f); // Wait for a short duration before placing the next spritePrefab

                totalSpritesSpawned++;

                if (totalSpritesSpawned == numSprites * 3)
                {
                    // All sprites spawned, trigger your audio and text
                    TriggerCompletion();
                }

                // Place new sprites inside existing sprites
                if (i < allFolderPaths.Count)
                {
                    string folderPath = allFolderPaths[i];
                    List<Sprite> spritesInFolder = LoadSpritesFromFolder(folderPath);
                    Debug.Log("Loaded Sprites in Folder: " + folderPath + ", Count: " + spritesInFolder.Count);

                    // Shuffle the list of sprites in the folder
                    ShuffleList(spritesInFolder);

                    if (spritesInFolder.Count > 0)
                    {
                        Sprite selectedSprite = GetUniqueSprite(spritesInFolder, usedSpritesAcrossLines, allFolderPaths); // Provide the missing argument

                        if (selectedSprite != null)
                        {
                            PlaceAdditionalSprite(spriteObject, selectedSprite);
                            usedSpritesInLine.Add(selectedSprite.name); // Track used sprites for this line
                        }
                        else
                        {
                            Debug.LogWarning("No unique sprite found in folder: " + folderPath);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No sprites found in folder: " + folderPath);
                    }
                }
                else
                {
                    Debug.LogWarning("Not enough folders for the specified number of sprites.");
                }
            }
            // Add used sprites in this line to the set of used sprites across all lines
            usedSpritesAcrossLines.AddRange(usedSpritesInLine);
        }
    }
    private void TriggerCompletion()
    {
        // Play completion audio
        if (completionAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(completionAudioClip, transform.position);
        }

        // Unhide completion text
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
        }

        // ... you can add any additional actions here
    }

    // Method to check if all additional sprites are spawned
    private bool AllAdditionalSpritesSpawned()
    {
        foreach (var additionalSprite in spawnedAdditionalSprites)
        {
            if (!additionalSprite.HasSpawnedSprite())
            {
                return false;
            }
        }
        return true;
    }

    // Method to be called when all additional sprites are spawned
    private void OnAllAdditionalSpritesSpawned()
    {
        // Your code to handle the event when all additional sprites are spawned
        Debug.Log("All additional sprites are spawned!");
        // You can play audio, show a message, or trigger any other actions.
    }

    private Sprite GetUniqueSprite(List<Sprite> sprites, List<string> usedSpritesAcrossLines, List<string> allFolderPaths)
    {
        ShuffleList(sprites); // Shuffle the list to randomize selection

        foreach (Sprite sprite in sprites)
        {
            // Check if the sprite name is not already used across all lines
            if (!usedSpritesAcrossLines.Contains(sprite.name))
            {
                usedSpritesAcrossLines.Add(sprite.name); // Mark the sprite as used
                return sprite;
            }
        }

        // If all sprites from the current folder are used, shuffle the list of folder paths and try again
        ShuffleList(allFolderPaths);

        foreach (string folderPath in allFolderPaths)
        {
            List<Sprite> spritesInFolder = LoadSpritesFromFolder(folderPath);

            ShuffleList(spritesInFolder);

            foreach (Sprite sprite in spritesInFolder)
            {
                if (!usedSpritesAcrossLines.Contains(sprite.name))
                {
                    usedSpritesAcrossLines.Add(sprite.name); // Mark the sprite as used
                    return sprite;
                }
            }
        }

        // If all sprites are used, return null
        return null;
    }


    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    private List<Sprite> LoadSpritesFromFolder(string folderPath)
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(folderPath);
        return new List<Sprite>(loadedSprites);
    }
  
    private void PlaceAdditionalSprite(GameObject parentObject, Sprite selectedSprite)
    {
        Debug.Log("PlaceAdditionalSprite called.");
        Debug.Log("Placing Additional Sprite: " + selectedSprite.name);

        // Additional debug messages
        Debug.Log("Parent Object: " + parentObject.name);
        Debug.Log("Parent Object Tag: " + parentObject.tag);

        // Instantiate the additional sprite object
        GameObject additionalSpriteObject = new GameObject("AdditionalSprite");
        additionalSpriteObject.transform.SetParent(parentObject.transform);

        // Add the marker script to the additional sprite object
        AdditionalSpriteMarkerForCreatureManager additionalSpriteMarker = additionalSpriteObject.AddComponent<AdditionalSpriteMarkerForCreatureManager>();

        // Assign the CheckMark prefab to the newSpritePrefab property
        additionalSpriteMarker.SetNewSpritePrefab(checkMarkPrefab);

        // Set the tag of the additional sprite object
        additionalSpriteObject.tag = "AdditionalSpriteTag";

        // Add sprite name to the list
        additionalSpriteNames.Add(selectedSprite.name);

        // Create a SpriteRenderer component for the additional sprite object
        SpriteRenderer additionalSpriteRenderer = additionalSpriteObject.AddComponent<SpriteRenderer>();
        additionalSpriteRenderer.sprite = selectedSprite;

        // Set the sorting layer of the additional sprite
        additionalSpriteRenderer.sortingLayerName = "Background"; // Assign the appropriate sorting layer

        // Add a BoxCollider2D to make the sprite clickable
        BoxCollider2D collider = additionalSpriteObject.AddComponent<BoxCollider2D>();
        collider.size = additionalSpriteRenderer.bounds.size; // Set collider size to match sprite size

        // Adjust the scale of the additional sprite to make it larger
        float scaleMultiplier = 30f; // Change this value to make the sprite bigger
        float parentWidth = parentObject.GetComponent<SpriteRenderer>().bounds.size.x;
        float parentHeight = parentObject.GetComponent<SpriteRenderer>().bounds.size.y;

        float additionalSpriteWidth = additionalSpriteRenderer.bounds.size.x;
        float additionalSpriteHeight = additionalSpriteRenderer.bounds.size.y;

        // Calculate the scaling factors for both width and height
        float scaleX = parentWidth * scaleMultiplier / additionalSpriteWidth;
        float scaleY = parentHeight * scaleMultiplier / additionalSpriteHeight;

        // Apply the scaling factors to the additional sprite
        additionalSpriteObject.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        float additionalSpriteX = 0f; // Centered by default
        float additionalSpriteY = 0f; // Centered by default
        float additionalSpriteZ = -0.03f; // Offset on the Z-axis

        additionalSpriteObject.transform.localPosition = new Vector3(additionalSpriteX, additionalSpriteY, additionalSpriteZ);   
    }
}