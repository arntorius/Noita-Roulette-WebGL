using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpriteGenerator : MonoBehaviour
{
    public GameObject[] spritePrefabs;
    public Transform container;
    public Button startButton;
    public Button stopButton;
    public int numberOfSprites = 100; // Number of sprites to generate
    public float wiggleAmount = 10f; // Amount of wiggle for the sprites
    public float wiggleInterval = 0.1f; // Interval between each wiggle update
    public float minDistance = .5f; // Minimum distance to maintain between sprites
    public float minDistanceFromCenter = 0.2f;
    private bool isRunning = false;

    private void Start()
    {
        startButton.onClick.AddListener(StartGeneration);
        stopButton.onClick.AddListener(StopGeneration);
        StartCoroutine(GenerateSprites());
        StartCoroutine(WiggleSprites());
    }
    private void StartGeneration()
    {
        if (spritePrefabs.Length == 0)
        {
            Debug.LogError("No sprite prefabs assigned!");
            return;
        }

        isRunning = true;
        StartCoroutine(GenerateSprites());
        StartCoroutine(WiggleSprites()); // Start the wiggling coroutine
    }
     private bool CheckOverlap(Vector3 position, float radius)
    {
        foreach (Transform child in container.transform)
        {
            float distance = Vector3.Distance(position, child.position);
            if (distance < radius)
            {
                return true;
            }
        }
        return false;
    }

    private void StopGeneration()
    {
        isRunning = false;
        DestroyGeneratedSprites();
    }

    private void OnDrawGizmosSelected()
    {
        if (container != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(container.position, container.localScale);
        }
    }
    private void DestroyGeneratedSprites()
    {
        // Destroy all sprite instances in the container
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator GenerateSprites()
    {
        float containerWidth = container.GetComponent<RectTransform>().rect.width;
        float containerHeight = container.GetComponent<RectTransform>().rect.height;
        int generatedSprites = 0;

        while (isRunning && generatedSprites < numberOfSprites)
        {
            GameObject randomSpritePrefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];
            GameObject spriteInstance = Instantiate(randomSpritePrefab, container);

            RectTransform rectTransform = spriteInstance.GetComponent<RectTransform>();

            float spriteWidth = rectTransform.rect.width;
            float spriteHeight = rectTransform.rect.height;

            float randomX = Random.Range(-containerWidth / 2f + spriteWidth / 2f, containerWidth / 2f - spriteWidth / 2f);
            float randomY = Random.Range(-containerHeight / 2f + spriteHeight / 2f, containerHeight / 2f - spriteHeight / 2f);

            // Check the distance from the center
            float distanceFromCenter = Vector2.Distance(new Vector2(randomX, randomY), Vector2.zero);
            if (distanceFromCenter >= minDistanceFromCenter)
            {
                rectTransform.anchoredPosition = new Vector2(randomX, randomY);
                generatedSprites++;
            }
            else
            {
                // If the sprite is too close to the center, destroy it and do not count it towards the generatedSprites count
                Destroy(spriteInstance);
            }

            yield return null;
        }
    }
    private IEnumerator WiggleSprites()
    {
        while (isRunning)
        {
            foreach (Transform child in container.transform)
            {
                // Generate random offsets for X and Y positions
                float offsetX = Random.Range(-wiggleAmount, wiggleAmount);
                float offsetY = Random.Range(-wiggleAmount, wiggleAmount);

                // Apply the offsets to the sprite's position
                Vector3 newPosition = child.transform.localPosition + new Vector3(offsetX, offsetY, 0f);
                child.transform.localPosition = newPosition;

                yield return null;
            }

            yield return new WaitForSeconds(wiggleInterval);
        }
    }

}
