using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class FlaskRaining : MonoBehaviour
{
    public GameObject flaskPrefab;
    public Transform flaskContainer;
    public Button[] startButtons;
    public Button[] stopButtons;
    public float spawnRate = 0.5f;
    public float minYPosition = 5f; // Minimum y position where the flasks can spawn
    public float maxYPosition = 15f; // Maximum y position where the flasks can spawn
    public float minXPosition = -10f; // Minimum x position where the flasks can spawn
    public float maxXPosition = 10f; // Maximum x position where the flasks can spawn
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 50f;
    public float minFallSpeed = 2f;
    public float maxFallSpeed = 5f;
    public float zPosition = 0.3828022f; // Z position for the flasks
    public float flaskLifetime = 5f; // Lifetime of the spawned flasks

    private float timer;
    private bool isRaining = false;
    private List<Sprite> flaskSprites = new List<Sprite>();

    void Start()
    {
        LoadFlaskSprites();

        foreach (Button startButton in startButtons)
        {
            startButton.onClick.AddListener(StartRaining);
        }

        foreach (Button stopButton in stopButtons)
        {
            stopButton.onClick.AddListener(StopRaining);
        }
    }

    void LoadFlaskSprites()
    {
        // Load all sprites from the "Flasks" folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Flasks");

        // Add loaded sprites to the flaskSprites list
        flaskSprites.AddRange(sprites);
    }

    void Update()
    {
        if (isRaining)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnFlask();
                timer = spawnRate;
            }
        }
    }

    void SpawnFlask()
    {
        // Calculate random spawn position within defined boundaries
        float randomX = Random.Range(minXPosition, maxXPosition);
        float randomY = Random.Range(minYPosition, maxYPosition);
        Vector3 spawnPosition = new Vector3(randomX, randomY, zPosition); // Set z position

        GameObject flask = Instantiate(flaskPrefab, spawnPosition, Quaternion.identity);
        flask.transform.SetParent(flaskContainer); // Set the flask's parent to the container.

        // Randomly select a sprite from the list of flaskSprites
        Sprite randomSprite = flaskSprites[Random.Range(0, flaskSprites.Count)];

        // Set the sprite of the flask
        flask.GetComponent<SpriteRenderer>().sprite = randomSprite;

        // Set scale of the flask
        flask.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);

        // Apply random rotation
        float rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        flask.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(0, 2) == 0 ? rotationSpeed : -rotationSpeed;

        // Apply random falling speed
        float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        flask.GetComponent<Rigidbody2D>().velocity = Vector2.down * fallSpeed;

        // Destroy the flask after a certain lifetime
        Destroy(flask, flaskLifetime);
    }

    void StartRaining()
    {
        isRaining = true;
    }

    void StopRaining()
    {
        isRaining = false;
    }
}
