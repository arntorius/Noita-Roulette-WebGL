using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AchievementPillars : MonoBehaviour
{
    public Button backButton;
    public Button rollButton;
    public Button stopRollButton;
    public Button reRollButton;
    public Button addPillarButton;
    public GameObject spriteContainer;
    public GameObject newContainer;
    public string spritePrefabsFolderPath = "Sprites/Achievement Cards/Prefabs";
    public AudioClip rollAudioClip;
    public float rollAudioPitch = 1.0f;

    private List<GameObject> spritePrefabs;
    private List<GameObject> availableSpritePrefabs; // New list to store available sprite prefabs
    private List<int> spriteWeights;
    private bool isRolling = false;
    private AudioSource rollAudioSource;
    private GameObject selectedSpritePrefab; // Added variable to store the selected sprite prefab
    private int pillarsAdded = 0;
    public int maxPillars = 2; // Maximum number of pillars allowed

    private int addPillarButtonPressCount = 0; // Counter to track the number of times "Add Pillar" button is pressed
    public Text notificationText;

    private float rollSpeed = 10f;
    private string displayedPrefabFilename; // Added variable to store the displayed prefab filename

    private void Start()
    {
        if (spriteContainer == null)
        {
            Debug.LogError("Sprite Container is not assigned!");
            return;
        }

        spritePrefabs = new List<GameObject>();
        availableSpritePrefabs = new List<GameObject>(); // Initialize available sprite prefabs list
        spriteWeights = new List<int>();

        rollAudioSource = gameObject.AddComponent<AudioSource>();
        rollAudioSource.loop = true;
        addPillarButton.interactable = (pillarsAdded < maxPillars);

        rollButton.onClick.AddListener(RollSprites);
        stopRollButton.onClick.AddListener(StopAndReset);
        backButton.onClick.AddListener(StopRollAndReset);
        reRollButton.onClick.AddListener(ReRollSprites);
        addPillarButton.onClick.AddListener(AddPillar);
    }

    private void StopRollAndReset()
    {
        StopAllAudioSources();
        StopRoll();
        ResetState();
        ClearNewContainer();
        addPillarButtonPressCount = 0;
        pillarsAdded = 0;
        isFirstPillarAdded = true;
    }

    private void StopAndReset()
    {
        StopAllAudioSources();
        StopRoll();
        ResetState();
    }

    private void ReRollSprites()
    {
        ClearNewContainer();
        addPillarButtonPressCount = 0;
        pillarsAdded = 0;
        isFirstPillarAdded = true;
        ResetState();
        StopRoll();
        StartCoroutine(RollAnimation());
    }

    private void ClearNewContainer()
    {
        foreach (Transform child in newContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private bool isFirstPillarAdded = true;
    private void AddPillar()
    {
        if (pillarsAdded >= maxPillars)
        {
            Debug.Log("Maximum number of pillars reached!");
            if (notificationText != null)
            {
                notificationText.text = "thats enough";
            }

            // Hide the stop button and show the reroll button
            stopRollButton.gameObject.SetActive(false);
            reRollButton.gameObject.SetActive(true);

            return;
        }

        addPillarButtonPressCount++;

        if (addPillarButtonPressCount >= 2)
        {
            addPillarButton.gameObject.SetActive(false);
        }

        if (notificationText != null)
        {
            notificationText.text = "";
        }

        GameObject displayedSprite = GetDisplayedSprite();

        if (displayedSprite != null)
        {
            GameObject newSpriteObject = Instantiate(displayedSprite, newContainer.transform);
            newSpriteObject.name = "SpriteObject";

            if (isFirstPillarAdded)
            {
                MoveSpriteToLeft(newSpriteObject);
            }
            else
            {
                MoveSpriteToRight(newSpriteObject);
            }

            ScaleDownSprite(newSpriteObject);

            availableSpritePrefabs.Remove(selectedSpritePrefab);
        }
        else
        {
            Debug.LogError("No sprite object found to duplicate!");
        }

        pillarsAdded++;
        isFirstPillarAdded = false;
        ResetState();
        StopRoll();
        StartCoroutine(RollAnimation());
    }


    private GameObject GetDisplayedSprite()
    {
        Transform spriteContainerTransform = spriteContainer.transform;
        if (spriteContainerTransform.childCount > 0)
        {
            return spriteContainerTransform.GetChild(0).gameObject;
        }
        return null;
    }

    private void ScaleSprite(GameObject spriteObject, Vector3 scale)
    {
        if (spriteObject != null)
        {
            spriteObject.transform.localScale = scale;
        }
    }

    private void MoveSpriteToRight(GameObject spriteObject)
    {
        if (spriteObject != null)
        {
            float moveAmount = 0.2f;
            spriteObject.transform.position += new Vector3(moveAmount, 0f, 0f);
        }
    }

    private void MoveSpriteToLeft(GameObject spriteObject)
    {
        if (spriteObject != null)
        {
            float moveAmount = -0.2f;
            spriteObject.transform.position += new Vector3(moveAmount, 0f, 0f);
        }
    }

    private void ScaleDownSprite(GameObject spriteObject)
    {
        if (spriteObject != null)
        {
            float scalePercentage = 0.7f;
            spriteObject.transform.localScale *= scalePercentage;
        }
    }

    private void AddStatEntry(string title, List<int> results, string filename)
    {
        // Add the result to the stats manager
        StatsManagerSingleton.Instance.AddRouletteStatEntry(title, results, filename);
    }

    private void StopAllAudioSources()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    private void ResetState()
    {
        isRolling = false;
        displayedPrefabFilename = null;
    }

    private void LoadSpritePrefabsFromFolder()
    {
        Object[] loadedPrefabs = Resources.LoadAll(spritePrefabsFolderPath, typeof(GameObject));

        foreach (Object obj in loadedPrefabs)
        {
            if (obj is GameObject)
            {
                spritePrefabs.Add(obj as GameObject);
                availableSpritePrefabs.Add(obj as GameObject); // Add to available sprite prefabs list
                spriteWeights.Add(spritePrefabs.Count);
            }
        }
    }

    private void RollSprites()
    {
        LoadSpritePrefabsFromFolder();

        if (availableSpritePrefabs.Count == 0)
        {
            Debug.LogError("No available sprite prefabs found!");
            return;
        }

        // Shuffle the available sprite prefabs list to introduce randomness
        Shuffle(availableSpritePrefabs);

        StartCoroutine(RollAnimation());
    }


    private void Shuffle<T>(List<T> list)
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

    private IEnumerator RollAnimation()
    {
        foreach (Transform child in spriteContainer.transform)
        {
            Destroy(child.gameObject);
        }

        isRolling = true;

        rollAudioSource.clip = rollAudioClip;
        rollAudioSource.pitch = rollAudioPitch;
        rollAudioSource.Play();

        while (isRolling)
        {
            float progress = Time.time * rollSpeed;

            int currentIndex = Mathf.FloorToInt(progress) % availableSpritePrefabs.Count;

            foreach (Transform child in spriteContainer.transform)
            {
                Destroy(child.gameObject);
            }

            selectedSpritePrefab = availableSpritePrefabs[currentIndex];

            DisplaySpritePrefabWithFrame(selectedSpritePrefab);

            yield return null;
        }

        rollAudioSource.Stop();
    }

    private void DisplaySpritePrefabWithFrame(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject spriteObject = Instantiate(prefab, spriteContainer.transform);
            spriteObject.name = "SpriteObject";

            Material frameMaterial = new Material(Shader.Find("Standard"));
            frameMaterial.color = Color.black;

            GameObject frameObject = new GameObject("FrameObject");
            frameObject.transform.SetParent(spriteObject.transform);
            frameObject.AddComponent<SpriteRenderer>().sprite = spriteObject.GetComponent<SpriteRenderer>().sprite;
            frameObject.GetComponent<SpriteRenderer>().material = frameMaterial;

            displayedPrefabFilename = spriteObject.name;
        }
        else
        {
            Debug.LogError("Invalid sprite prefab!");
        }
    }

    private void StopRoll()
    {
        isRolling = false;
        rollAudioSource.Stop();
    }
}
