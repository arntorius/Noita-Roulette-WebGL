using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PillarAnimation : MonoBehaviour
{
    public Button button; // Assign the button in the Inspector
    public int minNumberOfSprites = 3; // Minimum number of sprites
    public int maxNumberOfSprites = 5; // Maximum number of sprites
    public Transform spriteContainer; // Assign the container in the Inspector
    public string folderPath = "Sprites/AchievementPillars/PillarPrefab";
    public string secondFolderPath = "Sprites/AchievementPillars/BrokenPrefab"; // Add the second folder path
    public float fallDuration = 0.4f; // Duration for the falling animation

    private GameObject[] spritePrefabs;
    private GameObject[] secondSpritePrefabs;

    void Start()
    {
        LoadSpritePrefabs();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonPress);
        }
        else
        {
            Debug.LogError("Button component not found. Make sure the script is attached to a GameObject with a Button component.");
        }
    }

    void LoadSpritePrefabs()
    {
        // Load sprite prefabs from the first folder path
        Object[] prefabs = Resources.LoadAll(folderPath, typeof(GameObject));
        spritePrefabs = new GameObject[prefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
        {
            spritePrefabs[i] = prefabs[i] as GameObject;
        }

        // Load sprite prefabs from the second folder path
        Object[] secondPrefabs = Resources.LoadAll(secondFolderPath, typeof(GameObject));
        secondSpritePrefabs = new GameObject[secondPrefabs.Length];

        for (int i = 0; i < secondPrefabs.Length; i++)
        {
            secondSpritePrefabs[i] = secondPrefabs[i] as GameObject;
        }
    }

    void OnButtonPress()
    {
        StartCoroutine(SpawnSpritesAsync());
    }

    IEnumerator SpawnSpritesAsync()
    {
        // Clear existing sprites
        ClearSprites();

        float yOffset = 0.44f; // Add this line to declare yOffset

        // Generate a random number for the number of displayed sprites
        int numberOfSprites = Random.Range(minNumberOfSprites, Mathf.Min(maxNumberOfSprites, spritePrefabs.Length) + 1);

        for (int i = 0; i < numberOfSprites; i++)
        {
            // Load a random sprite prefab from the first folder path
            GameObject randomSpritePrefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];

            if (randomSpritePrefab != null)
            {
                // Specify starting position for each sprite
                float xOffset = 0.569999993f;
                float zOffset = 0.620000005f;
                Vector3 startPosition = new Vector3(xOffset, yOffset, zOffset);

                // Specify target position for each sprite
                float targetYOffset = -0.340000004f + i * 0.085f; // Adjust the offset as needed
                Vector3 targetPosition = new Vector3(xOffset, targetYOffset, zOffset);

                // Instantiate sprite at the starting position
                GameObject newSprite = Instantiate(randomSpritePrefab, startPosition, Quaternion.identity);
                newSprite.transform.parent = spriteContainer != null ? spriteContainer : transform;

                // Smoothly interpolate to the target position (falling animation)
                float elapsedTime = 0f;
                while (elapsedTime < fallDuration)
                {
                    newSprite.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / fallDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Ensure the sprite is at the exact target position
                newSprite.transform.position = targetPosition;
            }
            else
            {
                Debug.LogError("Failed to load random sprite prefab from the first folder path.");
            }

            // Introduce a delay before spawning the next sprite
            yield return new WaitForSeconds(fallDuration);
        }

        // Load a random sprite prefab from the second folder path
        GameObject secondRandomSpritePrefab = secondSpritePrefabs[Random.Range(0, secondSpritePrefabs.Length)];

        if (secondRandomSpritePrefab != null)
        {
            // Specify starting position for the second sprite
            float secondXOffset = 0.569999993f;
            float secondYOffset = -0.340000004f + numberOfSprites * 0.085f; // Place it above the very last sprite
            float secondZOffset = 0.620000005f;
            Vector3 secondStartPosition = new Vector3(secondXOffset, yOffset, secondZOffset); // Use the same yOffset as the first folder path

            // Specify target position for the second sprite
            float secondTargetYOffset = secondYOffset + 0.0f; // Adjust the offset as needed
            Vector3 secondTargetPosition = new Vector3(secondXOffset, secondTargetYOffset, secondZOffset);

            // Instantiate the second sprite at the starting position
            GameObject secondNewSprite = Instantiate(secondRandomSpritePrefab, secondStartPosition, Quaternion.identity);
            secondNewSprite.transform.parent = spriteContainer != null ? spriteContainer : transform;

            // Smoothly interpolate to the target position (falling animation)
            float secondElapsedTime = 0f;
            while (secondElapsedTime < fallDuration)
            {
                secondNewSprite.transform.position = Vector3.Lerp(secondStartPosition, secondTargetPosition, secondElapsedTime / fallDuration);
                secondElapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the second sprite is at the exact target position
            secondNewSprite.transform.position = secondTargetPosition;
        }
        else
        {
            Debug.LogError("Failed to load random sprite prefab from the second folder path.");
        }
    }


    void ClearSprites()
    {
        foreach (Transform child in spriteContainer != null ? spriteContainer : transform)
        {
            Destroy(child.gameObject);
        }
    }
}