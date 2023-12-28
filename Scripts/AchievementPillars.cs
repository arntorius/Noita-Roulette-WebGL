using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AchievementPillars : MonoBehaviour
{
    public Button backButton;
    public Button rollButton;
    public Button stopRollButton;
    public GameObject spriteContainer;
    public string spritePrefabsFolderPath = "Sprites/Achievement Cards/Prefabs";
    public AudioClip rollAudioClip;
    public float rollAudioPitch = 1.0f;

    private List<GameObject> spritePrefabs;
    private List<int> spriteWeights;
    private bool isRolling = false;
    private AudioSource rollAudioSource;
    private GameObject selectedSpritePrefab; // Added variable to store the selected sprite prefab


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
        spriteWeights = new List<int>();

        rollAudioSource = gameObject.AddComponent<AudioSource>();
        rollAudioSource.loop = true;

        rollButton.onClick.AddListener(RollSprites);
        stopRollButton.onClick.AddListener(StopAndReset);  // Use stopRollButton.onClick instead of backButton.onClick
        backButton.onClick.AddListener(StopAndReset);
    }
    private void StopAndReset()
    {
        StopAllAudioSources();
        StopRoll();
        ResetState();

        // Add the stat entry after stopping the roll
       // AddStatEntry("Achievement Pillars", new List<int> { GetDisplayedPrefabIndex() }, GetDisplayedPrefabFilename());
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
                spriteWeights.Add(spritePrefabs.Count);
            }
        }
    }

    private void RollSprites()
    {
        LoadSpritePrefabsFromFolder();

        if (spritePrefabs.Count == 0)
        {
            Debug.LogError("No sprite prefabs found in folder: " + spritePrefabsFolderPath);
            return;
        }

        Shuffle(spritePrefabs);

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

            int currentIndex = Mathf.FloorToInt(progress) % spritePrefabs.Count;

            foreach (Transform child in spriteContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Store the selected sprite prefab
            selectedSpritePrefab = spritePrefabs[currentIndex];

            DisplaySpritePrefabWithFrame(currentIndex);

            yield return null;
        }

        rollAudioSource.Stop();

        // After rolling is complete, add the stat entry with the filename
        // AddStatEntry("Achievement Pillars", new List<int> { GetDisplayedPrefabIndex() }, GetDisplayedPrefabFilename());
    }


    private int WeightedRandomIndex()
    {
        int totalWeight = 0;

        foreach (int weight in spriteWeights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);

        int currentIndex = 0;

        while (randomValue >= spriteWeights[currentIndex])
        {
            randomValue -= spriteWeights[currentIndex];
            currentIndex++;

            if (currentIndex >= spriteWeights.Count)
            {
                currentIndex = 0;
                break;
            }
        }

        return currentIndex;
    }

    private void DisplaySpritePrefabWithFrame(int index)
    {
        if (index >= 0 && index < spritePrefabs.Count)
        {
            GameObject spriteObject = Instantiate(spritePrefabs[index], spriteContainer.transform);
            spriteObject.name = "SpriteObject";

            Material frameMaterial = new Material(Shader.Find("Standard"));
            frameMaterial.color = Color.black;

            GameObject frameObject = new GameObject("FrameObject");
            frameObject.transform.SetParent(spriteObject.transform);
            frameObject.AddComponent<SpriteRenderer>().sprite = spriteObject.GetComponent<SpriteRenderer>().sprite;
            frameObject.GetComponent<SpriteRenderer>().material = frameMaterial;

            // Set the displayed prefab filename
            displayedPrefabFilename = spriteObject.name;
        }
        else
        {
            Debug.LogError("Invalid sprite prefab index: " + index);
        }
    }


    private void StopRoll()
    {
        isRolling = false;
        rollAudioSource.Stop();
    }

    private int GetDisplayedPrefabIndex()
    {
        // Find the index of the displayed prefab in the original list
        int index = spritePrefabs.FindIndex(obj => obj.name == selectedSpritePrefab.name);
        return index;
    }

    private string GetDisplayedPrefabFilename()
    {
        // Extract the filename of the displayed prefab
        if (spriteContainer.transform.childCount > 0)
        {
            Transform child = spriteContainer.transform.GetChild(0);
            if (child != null)
            {
                SpriteRenderer spriteRenderer = child.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    string filename = spriteRenderer.sprite.name;
                    Debug.Log("Displayed Filename: " + filename);
                    return filename;
                }
            }
        }

        Debug.LogError("Could not get displayed prefab filename!");
        return null;
    }

}

