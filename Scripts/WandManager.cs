using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Text;

[System.Serializable]
public class FolderData
{
    public string folderName;
    public string folderPath; // Add folder path property
    public int weight;
    public int folderIndex; // Add folder index property
}

public class WandManager : MonoBehaviour
{
    public Button spawnButton;
    public Button rerollButton;
    public Text rerollButtonText; // Reference to the Text component of the reroll button
    public Button destroyButton;
    public Button printButton;
    public Transform container;
    public AudioClip audioClip;
    public Text resultText;
    public Text bansLeftText; // Reference to the Text component in the Unity Inspector
    public float spacing = 1f;
    public List<FolderData> folders;
    private int numSprites; // Variable to store the number of sprites for reroll
    private int rerollCount = 3; // Number of allowed rerolls
    private int currentReroll = 0; // Counter for the current reroll attempts
    private int spritePrefab2Count = 0;
    private int spritePrefab2Limit = 3; // Set the limit to 3



    [SerializeField]
    private GameObject spritePrefab;
    [SerializeField]
    private GameObject spritePrefab2; // Serialize this field to make it visible in the Unity Inspector
    [SerializeField]
    private float spritePrefab2ScaleFactor = 1f; // Serialize this field to make it visible in the Unity Inspector

    [SerializeField]
    private Text additionalSpritesTextField; // Reference to the Text component in the Unity Inspector

    [Header("Formatting Options")]
    [SerializeField]
    private int rows = 5; // Number of rows for the formatted text

    [SerializeField]
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
        UpdateBansLeftText();
    }
    private void OnResetButtonClick()
    {
        UpdateRerollButtonText(); // Update the reroll button text at the start
        // Reset the spritePrefab2 count to -3
        spritePrefab2Count = -3;
        Debug.Log("SpritePrefab2 Count Reset to: " + spritePrefab2Count);
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

        // Reset the spritePrefab2 count to 0 when reroll button is pressed
        spritePrefab2Count = 0; // Reset the counter here
        Debug.Log("SpritePrefab2 Count Reset to: " + spritePrefab2Count);

        UpdateBansLeftText(); // Update the bans left text after resetting the count

        // Reset the rerollCount count to 0 when reroll button is pressed
        rerollCount = 3; // Reset the counter here
        Debug.Log("rerollCount Count Reset to: " + rerollCount);

        UpdateRerollButtonText(); // Update the rerolls left text after resetting the count
    }

    private void OnRerollButtonPress()
    {
        if (currentReroll < rerollCount)
        {
            // Destroy all existing sprites before rerolling
            DestroyAllSprites();

            // Reroll with the same number of sprites as the initial roll
            StartCoroutine(PlaceSpritePrefabs(numSprites));
            resultText.text = "Number of Slots: " + numSprites.ToString();

            currentReroll++; // Increment the reroll counter
            if (currentReroll >= rerollCount)
            {
                rerollButton.interactable = true; // Disable the reroll button after reaching the allowed rerolls
            }

            UpdateRerollButtonText(); // Update the reroll button text after each reroll attempt

            // Update the bans left text field
            int remainingBans = FindObjectOfType<WandManager>().GetRemainingBans();
            bansLeftText.text = remainingBans + " Bans Left";
        }
        else
        {
            // Display a message indicating no more rerolls are allowed
            resultText.text = "No more rerolls allowed!";
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

        // Reset the spritePrefab2 count to 0 when reroll button is pressed
        spritePrefab2Count = 0; // Reset the counter here
        Debug.Log("SpritePrefab2 Count Reset to: " + spritePrefab2Count);

        UpdateBansLeftText(); // Update the bans left text after resetting the count
    }

    public int GetRemainingBans()
    {
        int remainingBans = spritePrefab2Limit - spritePrefab2Count;
        return Mathf.Max(remainingBans, 0); // Ensure the remaining bans are non-negative
    }
    private void UpdateBansLeftText()
    {
        int bansLeft = spritePrefab2Limit - spritePrefab2Count;

        if (bansLeft > 1)
        {
            bansLeftText.text = bansLeft + " Bans Left";
        }
        else if (bansLeft == 1)
        {
            bansLeftText.text = "1 Ban Left";
        }
        else
        {
            bansLeftText.text = "Good Luck";
        }
    }

    public void IncrementSpritePrefab2Count()
    {
        spritePrefab2Count++;
        Debug.Log("Current SpritePrefab2 Count: " + spritePrefab2Count);

        // Update bans left text when spritePrefab2 count is incremented
        UpdateBansLeftText();
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
            rerollButtonText.text = "1 Re-Roll Left";
        }
        else
        {
            rerollButtonText.text = "Good Luck";
        }
    }
    private void OnSpawnButtonPress()
    {
        numSprites = Random.Range(7, 21);
        StartCoroutine(PlaceSpritePrefabs(numSprites));
        resultText.text = "Number of Slots: " + numSprites.ToString();
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
        AdditionalSpriteMarker[] additionalSprites = FindObjectsOfType<AdditionalSpriteMarker>();

        foreach (AdditionalSpriteMarker additionalSprite in additionalSprites)
        {
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

            string spriteNamesText = "Spells:\n";

            int count = 0;
            foreach (string spriteName in additionalSpriteNames)
            {
                spriteNamesText += spriteName + "\t"; // Use '\t' for tab spacing

                count++;
                if (count % columns == 0)
                {
                    spriteNamesText += "\n"; // Start a new row after 'columns' number of items
                }
                else
                {
                    spriteNamesText += "/ "; // Add a space between sprite names within the same row
                }

                if (count >= rows * columns)
                {
                    break; // Limit the displayed items to rows * columns
                }
            }

            additionalSpritesTextField.text = spriteNamesText;
        }
    }
    void Update()
    {
        // Check for right mouse button click
        if (Input.GetMouseButtonDown(1)) // 1 represents the right mouse button
        {
            HandleRightMouseClick();
        }
    }
    private void HandleRightMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast a ray from the mouse position
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the "SpritePrefab2" tag
            if (hit.collider.gameObject.CompareTag("SpritePrefab2"))
            {
                // Destroy the SpritePrefab2
                Destroy(hit.collider.gameObject);

                // Decrement the count of spritePrefab2 instances
                spritePrefab2Count--;

                Debug.Log("Current SpritePrefab2 Count: " + spritePrefab2Count);

                // Update the UI to reflect the changes (if needed)
                DisplaySpriteNamesOnScreen(); // Assuming you have a function to update the displayed sprite names
            }
        }
    }

    private IEnumerator PlaceSpritePrefabs(int numSprites)
    {
        float totalWidth = 0f;
        List<string> allFolderPaths = new List<string>();

        foreach (var folder in folders)
        {
            for (int i = 0; i < folder.weight; i++)
            {
                allFolderPaths.Add(folder.folderPath);
            }
        }

        // Shuffle the list of folder paths
        ShuffleList(allFolderPaths);

        // Handle the special case for the first slot (index 0) only if there are more than 1 sprites
        if (numSprites > 1)
        {
            GameObject firstSpriteObject = Instantiate(spritePrefab, container);
            SpriteRenderer firstSpriteRenderer = firstSpriteObject.GetComponent<SpriteRenderer>();
            string firstFolderPath = "Sprites/Projectile Spells"; // Replace this with the actual folder path
            List<Sprite> firstSpritesInFolder = LoadSpritesFromFolder(firstFolderPath);

            // Shuffle the list of sprites in the folder
            ShuffleList(firstSpritesInFolder);

            // Check if there are sprites in the folder before accessing them
            if (firstSpritesInFolder.Count > 0)
            {
                Sprite firstSelectedSprite = firstSpritesInFolder[0]; // Select the first sprite after shuffling
                PlaceAdditionalSprite(firstSpriteObject, firstSelectedSprite);
            }
            else
            {
                Debug.LogError("No sprites found in the specified folder: " + firstFolderPath);
                // Handle this situation as needed, for example, by assigning a default sprite
            }

            totalWidth += firstSpriteRenderer.bounds.size.x + spacing;
            yield return new WaitForSeconds(0.1f); // Wait for a short duration before placing the next spritePrefab
        }
        else
        {
            // If only one sprite, instantiate the spritePrefab without any additional sprite
            Instantiate(spritePrefab, container);
            yield break; // Exit the coroutine since there are no more sprites to place
        }

        // Handle the rest of the slots (from index 1 to numSprites)
        for (int i = 1; i < numSprites; i++)
        {
            GameObject spriteObject = Instantiate(spritePrefab, container);
            SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
            float spriteWidth = spriteRenderer.bounds.size.x;

            totalWidth += spriteWidth + spacing;

            float startX = -totalWidth / 2f;

            for (int j = 0; j < i; j++)
            {
                Transform existingSprite = container.GetChild(j);
                float offset = startX + j * spriteWidth + j * spacing;
                Vector3 newPosition = new Vector3(offset, 0, 0);
                existingSprite.localPosition = newPosition;
            }

            float spriteX = startX + i * spriteWidth + i * spacing;
            spriteObject.transform.localPosition = new Vector3(spriteX, 0, 0);

            AudioSource.PlayClipAtPoint(audioClip, spriteObject.transform.position);

            yield return new WaitForSeconds(0.1f); // Wait for a short duration before placing the next spritePrefab

            // Place new sprites inside existing sprites
            if (i < allFolderPaths.Count)
            {
                string folderPath = allFolderPaths[i];
                List<Sprite> spritesInFolder = LoadSpritesFromFolder(folderPath);

                // Shuffle the list of sprites in the folder
                ShuffleList(spritesInFolder);

                if (spritesInFolder.Count > 0)
                {
                    Sprite selectedSprite = spritesInFolder[0]; // Select the first sprite after shuffling
                    PlaceAdditionalSprite(spriteObject, selectedSprite);
                }
            }
        }
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
    public void SetSpritePrefab2Scale()
    {
        SetSpritePrefab2Scale(spritePrefab2ScaleFactor);
    }
    public void SetSpritePrefab2Scale(float scaleFactor)
    {
        // Check if spritePrefab2 is not null before attempting to scale
        if (spritePrefab2 != null)
        {
            // Get the SpriteRenderer component of spritePrefab2
            SpriteRenderer spriteRenderer = spritePrefab2.GetComponent<SpriteRenderer>();

            // Check if the SpriteRenderer component exists
            if (spriteRenderer != null)
            {
                // Calculate new scale based on the original sprite size and the desired scale factor
                float newWidth = spriteRenderer.sprite.bounds.size.x * scaleFactor;
                float newHeight = spriteRenderer.sprite.bounds.size.y * scaleFactor;

                // Apply the new scale to the spritePrefab2
                spritePrefab2.transform.localScale = new Vector3(newWidth / spriteRenderer.sprite.bounds.size.x,
                                                                 newHeight / spriteRenderer.sprite.bounds.size.y,
                                                                 1f);
            }
            else
            {
                Debug.LogWarning("SpriteRenderer component not found on spritePrefab2.");
            }
        }
        else
        {
            Debug.LogWarning("spritePrefab2 is not assigned. Please assign the sprite prefab in the Inspector.");
        }
    }
    private void PlaceAdditionalSprite(GameObject parentObject, Sprite selectedSprite)
    {
        Debug.Log("PlaceAdditionalSprite called.");
        Debug.Log("Placing Additional Sprite: " + selectedSprite.name);

        // Check if the limit has been reached
        if (GetSpritePrefab2Count() >= spritePrefab2Limit)
        {
            Debug.Log("Cannot generate more spritePrefab2 instances. Limit reached.");
            return;
        }

        // Instantiate the additional sprite object
        GameObject additionalSpriteObject = new GameObject("AdditionalSprite");
        additionalSpriteObject.transform.SetParent(parentObject.transform);

        // Add the marker script to the additional sprite object
        AdditionalSpriteMarker additionalSpriteMarker = additionalSpriteObject.AddComponent<AdditionalSpriteMarker>();

        // Assign the new sprite prefab to the AdditionalSpriteMarker component
        additionalSpriteMarker.SetNewSpritePrefab(spritePrefab2);

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

        // Check if any child of parentObject has the tag "SpritePrefab2"
        bool hasSpritePrefab2Child = false;
        foreach (Transform child in additionalSpriteObject.transform)
        {
            if (child.CompareTag("SpritePrefab2"))
            {
                hasSpritePrefab2Child = true;
                break;
            }
        }

        if (hasSpritePrefab2Child)
        {
            // Increment the count of spritePrefab2 instances
            spritePrefab2Count++;
            Debug.Log("Current SpritePrefab2 Count: " + spritePrefab2Count);
        }
        SetSpritePrefab2SortingLayer("Foreground");
    }
    public void SetSpritePrefab2SortingLayer(string sortingLayerName)
    {
        // Check if spritePrefab2 is not null before attempting to set the sorting layer
        if (spritePrefab2 != null)
        {
            // Get the SpriteRenderer component of spritePrefab2
            SpriteRenderer spriteRenderer = spritePrefab2.GetComponent<SpriteRenderer>();

            // Check if the SpriteRenderer component exists
            if (spriteRenderer != null)
            {
                // Set the sorting layer of spritePrefab2
                spriteRenderer.sortingLayerName = sortingLayerName;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer component not found on spritePrefab2.");
            }
        }
        else
        {
            Debug.LogWarning("spritePrefab2 is not assigned. Please assign the sprite prefab in the Inspector.");
        }
    }
    public int GetSpritePrefab2Count()
    {
        return spritePrefab2Count;
    }

    public int GetSpritePrefab2Limit()
    {
        return spritePrefab2Limit;
    }

}