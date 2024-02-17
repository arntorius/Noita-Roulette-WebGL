using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PillarAnimation : MonoBehaviour
{
    public Button stopButton; // Assign the button in the Inspector
    public Button rerollButton;
    public Button backButton;
    public int minNumberOfSprites = 3; // Minimum number of sprites
    public int maxNumberOfSprites = 5; // Maximum number of sprites
    public Transform spriteContainer; // Assign the container in the Inspector
    public string folderPath = "Sprites/AchievementPillars/PillarPrefab";
    public string secondFolderPath = "Sprites/AchievementPillars/BrokenPrefab"; // Add the second folder path
    public float fallDuration = 0.4f; // Duration for the falling animation

    private GameObject[] spritePrefabs;
    private GameObject[] secondSpritePrefabs;
    private Coroutine spawnCoroutine; // Reference to the coroutine responsible for spawning sprites
    private float xOffset = 0.569999993f;
    private int rolloutCount = 0;
    private bool isRolloutInProgress = false;

    void Start()
    {
        LoadSpritePrefabs();

        if (stopButton != null)
        {
            stopButton.onClick.AddListener(OnStopButtonPress);
        }
        else
        {
            Debug.LogError("Stop Button component not found. Make sure the script is attached to a GameObject with a Button component.");
        }

        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(OnRerollButtonPress);
        }
        else
        {
            Debug.LogError("Reroll Button component not found. Make sure the script is attached to a GameObject with a Button component.");
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnRerollButtonPress);
        }
        else
        {
            Debug.LogError("Back Button component not found. Make sure the script is attached to a GameObject with a Button component.");
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

    void OnStopButtonPress()
    {
        if (!isRolloutInProgress)
        {
            Vector3 offset;
            if (rolloutCount == 0)
                offset = Vector3.zero;
            else if (rolloutCount == 1)
                offset = new Vector3(-0.33f, 0f, 0f);
            else
                offset = new Vector3(-0.8f, 0f, 0f);

            spawnCoroutine = StartCoroutine(SpawnSpritesAsync(offset));
            rolloutCount++;
        }
    }

    void OnRerollButtonPress()
    {
        if (isRolloutInProgress)
        {
            StopCoroutine(spawnCoroutine);
            ClearSprites();
            rolloutCount = 0;
            isRolloutInProgress = false;
        }
        ClearSprites();
        rolloutCount = 0;
    }

    IEnumerator SpawnSpritesAsync(Vector3 offset)
    {
        isRolloutInProgress = true;

        float yOffset = 0.44f;

        int numberOfSprites = Random.Range(minNumberOfSprites, Mathf.Min(maxNumberOfSprites, spritePrefabs.Length) + 1);

        for (int i = 0; i < numberOfSprites; i++)
        {
            GameObject randomSpritePrefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];

            if (randomSpritePrefab != null)
            {
                float zOffset = 0.620000005f;
                Vector3 startPosition = new Vector3(xOffset, yOffset, zOffset) + offset;

                float targetYOffset = -0.340000004f + i * 0.085f;
                Vector3 targetPosition = new Vector3(xOffset, targetYOffset, zOffset) + offset;

                GameObject newSprite = Instantiate(randomSpritePrefab, startPosition, Quaternion.identity);
                newSprite.transform.parent = spriteContainer != null ? spriteContainer : transform;

                float elapsedTime = 0f;
                while (elapsedTime < fallDuration)
                {
                    newSprite.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / fallDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                newSprite.transform.position = targetPosition;
            }
            else
            {
                Debug.LogError("Failed to load random sprite prefab from the first folder path.");
            }

            yield return new WaitForSeconds(fallDuration);
        }

        GameObject secondRandomSpritePrefab = secondSpritePrefabs[Random.Range(0, secondSpritePrefabs.Length)];

        if (secondRandomSpritePrefab != null)
        {
            float secondYOffset = -0.340000004f + numberOfSprites * 0.085f;
            float secondZOffset = 0.620000005f;
            Vector3 secondStartPosition = new Vector3(xOffset, yOffset, secondZOffset) + offset;

            float secondTargetYOffset = secondYOffset + 0.0f;
            Vector3 secondTargetPosition = new Vector3(xOffset, secondTargetYOffset, secondZOffset) + offset;

            GameObject secondNewSprite = Instantiate(secondRandomSpritePrefab, secondStartPosition, Quaternion.identity);
            secondNewSprite.transform.parent = spriteContainer != null ? spriteContainer : transform;

            float secondElapsedTime = 0f;
            while (secondElapsedTime < fallDuration)
            {
                secondNewSprite.transform.position = Vector3.Lerp(secondStartPosition, secondTargetPosition, secondElapsedTime / fallDuration);
                secondElapsedTime += Time.deltaTime;
                yield return null;
            }

            secondNewSprite.transform.position = secondTargetPosition;
        }
        else
        {
            Debug.LogError("Failed to load random sprite prefab from the second folder path.");
        }

        isRolloutInProgress = false;
    }

    void ClearSprites()
    {
        foreach (Transform child in spriteContainer != null ? spriteContainer : transform)
        {
            Destroy(child.gameObject);
        }
    }
}
