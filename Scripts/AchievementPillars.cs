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
    public Button reRollButtonSingle;
    public Button addPillarButton;
    public GameObject spriteContainer;
    public GameObject newContainer;
    public string spritePrefabsFolderPath = "Sprites/Achievement Cards/Prefabs";
    public AudioClip rollAudioClip;
    public float rollAudioPitch = 1.0f;
    public ClickedStateData clickedStateData; // Reference to the ClickedStateData scriptable object

    private List<GameObject> spritePrefabs;
    private List<GameObject> availableSpritePrefabs;
    private List<int> spriteWeights;
    private bool isRolling = false;
    private AudioSource rollAudioSource;
    private GameObject selectedSpritePrefab;
    private int pillarsAdded = 0;
    public int maxPillars = 2;
    private int addPillarButtonPressCount = 0;
    public Text notificationText;
    private float rollSpeed = 20f;
    private string displayedPrefabFilename;
    private bool isFirstPillarAdded = true;

    private void Start()
    {
        if (spriteContainer == null)
        {
            Debug.LogError("Sprite Container is not assigned!");
            return;
        }
        spritePrefabs = new List<GameObject>();
        availableSpritePrefabs = new List<GameObject>();
        spriteWeights = new List<int>();

        rollAudioSource = gameObject.AddComponent<AudioSource>();
        rollAudioSource.loop = true;
        addPillarButton.interactable = (pillarsAdded < maxPillars);

        rollButton.onClick.AddListener(RollSprites);
        reRollButtonSingle.onClick.AddListener(RerollLastSprite);
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

        // Clear the available sprite prefabs list and reload all unclicked sprite prefabs
        availableSpritePrefabs.Clear();
        LoadSpritePrefabsFromFolder();

        StartCoroutine(RollAnimation());
    }
    private void RerollLastSprite()
    {
        if (availableSpritePrefabs.Count == 0)
        {
            Debug.LogError("No available sprite prefabs found!");
            return;
        }

        // Get the last sprite displayed on the screen
        GameObject lastSprite = GetDisplayedSprite();
        if (lastSprite != null)
        {
            // Remove the last sprite from the screen
            Destroy(lastSprite);

            // Roll a new sprite and display it at the same position
            int newIndex = Random.Range(0, availableSpritePrefabs.Count);
            selectedSpritePrefab = availableSpritePrefabs[newIndex];
            DisplaySpritePrefabWithoutFrame(selectedSpritePrefab);

            // Trigger the roll animation
            StartCoroutine(RollAnimation());
        }
        else
        {
            Debug.LogError("Last sprite not found!");
        }
    }

    private void ClearNewContainer()
    {
        foreach (Transform child in newContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddPillar()
    {
        if (pillarsAdded >= maxPillars)
        {
            Debug.Log("Maximum number of pillars reached!");
            if (notificationText != null)
            {
                notificationText.text = "That's enough";
            }

            stopRollButton.gameObject.SetActive(false);
            reRollButton.gameObject.SetActive(true);
            reRollButtonSingle.gameObject.SetActive(true);

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
            newSpriteObject.name = displayedSprite.name; // Set the name to the original sprite prefab name

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
                GameObject prefab = obj as GameObject;
                string spriteName = prefab.name;
                if (!clickedStateData.GetClickedState(spriteName)) // Exclude clicked sprites
                {
                    spritePrefabs.Add(prefab);
                    availableSpritePrefabs.Add(prefab);
                    spriteWeights.Add(spritePrefabs.Count);
                }
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
        // Destroy any existing sprite objects in the container
        foreach (Transform child in spriteContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Set rolling flag to true
        isRolling = true;

        // Play the roll audio clip
        rollAudioSource.clip = rollAudioClip;
        rollAudioSource.pitch = rollAudioPitch;
        rollAudioSource.Play();

        // Disable colliders while rolling
        SetCollidersEnabled(false);

        // Loop while rolling
        while (isRolling)
        {
            float progress = Time.time * rollSpeed;
            Debug.Log("Progress: " + progress + ", Roll Speed: " + rollSpeed);

            int currentIndex = Mathf.FloorToInt(progress) % availableSpritePrefabs.Count;

            foreach (Transform child in spriteContainer.transform)
            {
                Destroy(child.gameObject);
            }

            selectedSpritePrefab = availableSpritePrefabs[currentIndex];

            DisplaySpritePrefabWithoutFrame(selectedSpritePrefab);

            yield return null;
        }


        // Re-enable colliders after rolling
        SetCollidersEnabled(true);

        // Stop the roll audio
        rollAudioSource.Stop();
    }

    private void DisplaySpritePrefabWithoutFrame(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject spriteObject = Instantiate(prefab, spriteContainer.transform);
            spriteObject.name = prefab.name; // Set the name to the original sprite prefab name
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

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (var spritePrefab in availableSpritePrefabs)
        {
            Collider2D collider = spritePrefab.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }
}
