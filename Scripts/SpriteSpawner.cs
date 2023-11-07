using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSpawner : MonoBehaviour
{
    public float spawnInterval = 5f;
    public Button spawnButton;
    public Button resetButton; // Add a public reset button variable in the inspector
    public Transform spawnContainer; // Assign the container in the inspector

    public float spriteScale = 1f;
    public Vector2 offsetRange = new Vector2(-2f, 2f); // Min and max offset values in inspector

    private int spawnCount = 0; // Counter for spawned sprites

    private void Start()
    {
        spawnButton.onClick.AddListener(StartSpawning);
        resetButton.onClick.AddListener(ResetSprites);
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnSpritesWithInterval());
    }

    private IEnumerator SpawnSpritesWithInterval()
    {
        while (spawnCount < 20)
        {
            SpawnSprite();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSprite()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Wands");
        if (sprites.Length > 0 && spawnCount < 20)
        {
            spawnCount++;

            Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];

            GameObject spriteObject = new GameObject("SpriteObject");
            spriteObject.transform.parent = spawnContainer;
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = randomSprite;

            spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);

            float offsetX = Random.Range(offsetRange.x, offsetRange.y);
            float offsetY = Random.Range(offsetRange.x, offsetRange.y);

            Vector3 spawnPosition = new Vector3(
                spawnContainer.position.x + offsetX,
                spawnContainer.position.y + offsetY,
                spawnContainer.position.z
            );

            spriteObject.transform.position = spawnPosition;

            StartCoroutine(MoveSprite(spriteObject));
            StartCoroutine(RotateSprite(spriteObject));
        }
        else
        {
            Debug.LogError("No sprites found in the specified folder: Resources/Sprites/Wands");
        }
    }

    private IEnumerator MoveSprite(GameObject spriteObject)
    {
        float speed = 0.1f;

        while (spriteObject != null)
        {
            spriteObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (spriteObject.transform.position.y < -10f)
            {
                Destroy(spriteObject);
            }

            yield return null;
        }
    }

    private IEnumerator RotateSprite(GameObject spriteObject)
    {
        float rotationSpeed = 180f;
        while (spriteObject != null)
        {
            spriteObject.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void ResetSprites()
    {
        // Destroy all child objects in the spawn container
        foreach (Transform child in spawnContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Reset spawn count
        spawnCount = 0;
    }
}
