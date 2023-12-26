using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AchievementPillars : MonoBehaviour
{
    public Button backButton;
    public Button rollButton;
    public Button stopRollButton; // Assignable in Inspector
    public GameObject spriteContainer;
    public string spritePrefabsFolderPath = "Sprites/Achievement Cards/Prefabs";
    public AudioClip rollAudioClip; // Assignable in Inspector
    public float rollAudioPitch = 1.0f; // Adjust this pitch in the inspector

    private List<GameObject> spritePrefabs;
    private List<int> spriteWeights;
    private bool isRolling = false;
    private AudioSource rollAudioSource;

    private float rollSpeed = 10f; // Adjust this value to control the roll speed directly

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
        stopRollButton.onClick.AddListener(StopRoll);
        backButton.onClick.AddListener(StopAndReset);
    }
    private void StopAndReset()
    {
        StopAllAudioSources();
        StopRoll();
        ResetState();
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
        // Reset any state variables or perform cleanup here
        isRolling = false;
       
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

        // Shuffle the list to ensure a random order
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

            DisplaySpritePrefabWithFrame(currentIndex);

            yield return null;
        }

        rollAudioSource.Stop();
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

            // Add a one-pixel black shaded frame
            Material frameMaterial = new Material(Shader.Find("Standard"));
            frameMaterial.color = Color.black;

            GameObject frameObject = new GameObject("FrameObject");
            frameObject.transform.SetParent(spriteObject.transform);
            frameObject.AddComponent<SpriteRenderer>().sprite = spriteObject.GetComponent<SpriteRenderer>().sprite;
            frameObject.GetComponent<SpriteRenderer>().material = frameMaterial;
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
}
