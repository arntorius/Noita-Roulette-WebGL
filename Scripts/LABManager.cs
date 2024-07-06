using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LABManager : MonoBehaviour
{
    public Button RollButton;
    public Button BackButton;
    public Button FillButton;
    public Transform ParentTransform;
    public int MinPrefabs = 1;
    public int MaxPrefabs = 5;
    public int PrefabsPerRow = 15;
    public float PrefabSpacing = 50f;
    public Vector3 PrefabScale = new Vector3(0.05f, 0.05f, 0.05f);
    public float PrefabZPosition = -0.3f;
    public float RowOffset = 50f;
    public float SlideInDuration = 0.5f;
    public float SlideInDelay = 0.1f;
    public float FillDelay = 0.5f;
    public Text ResultText;
    public AudioSource audioSource; // Reference to the AudioSource component

    public GameObject[] liquidPrefabs;
    public GameObject textLabelPrefab; // Drag your Text prefab here in the Inspector

    private List<GameObject> loadedPrefabs = new List<GameObject>();
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private List<GameObject> usedLiquidPrefabs = new List<GameObject>();

    void Start()
    {
        loadedPrefabs.AddRange(Resources.LoadAll<GameObject>("Sprites/LAB/LABPrefabs"));
        liquidPrefabs = Resources.LoadAll<GameObject>("Sprites/LAB/Liquids");

        if (RollButton != null)
        {
            RollButton.onClick.AddListener(OnRollButtonClicked);
        }
        if (BackButton != null)
        {
            BackButton.onClick.AddListener(OnBackButtonClicked);
        }
        if (FillButton != null)
        {
            FillButton.onClick.AddListener(OnFillButtonClicked);
            FillButton.gameObject.SetActive(false);
        }
    }

    void OnRollButtonClicked()
    {
        ClearPrefabs();

        int numberOfPrefabs = UnityEngine.Random.Range(MinPrefabs, MaxPrefabs + 1); // Use UnityEngine.Random here
        int numberOfRows = Mathf.CeilToInt((float)numberOfPrefabs / PrefabsPerRow);
        float totalHeight = (numberOfRows - 1) * RowOffset;
        float startY = totalHeight / 2;
        float totalWidth = (Mathf.Min(PrefabsPerRow, numberOfPrefabs) - 1) * PrefabSpacing;
        float startX = -totalWidth / 2;

        // Play the assigned audio clip
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        StartCoroutine(SlideInPrefabs(numberOfPrefabs, startX, startY));
    }

    IEnumerator SlideInPrefabs(int numberOfPrefabs, float startX, float startY)
    {
        Vector3 previousPosition = new Vector3(startX, startY, PrefabZPosition);

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            if (i > 0 && i % PrefabsPerRow == 0)
            {
                startY -= RowOffset;
                float totalWidth = (Mathf.Min(PrefabsPerRow, numberOfPrefabs - i) - 1) * PrefabSpacing;
                startX = -totalWidth / 2;
                previousPosition = new Vector3(startX, startY, PrefabZPosition);
            }

            GameObject prefab = loadedPrefabs[UnityEngine.Random.Range(0, loadedPrefabs.Count)]; // Use UnityEngine.Random here
            GameObject newPrefab = Instantiate(prefab, ParentTransform);
            newPrefab.transform.localScale = PrefabScale;

            Vector3 targetPosition = new Vector3(startX + (i % PrefabsPerRow) * PrefabSpacing, startY, PrefabZPosition);
            newPrefab.transform.localPosition = previousPosition;
            StartCoroutine(SlideInAnimation(newPrefab, targetPosition));

            // Instantiate text label prefab only for liquid prefabs
            if (System.Array.Exists(liquidPrefabs, element => element == prefab))
            {
                GameObject newLabel = Instantiate(textLabelPrefab, ParentTransform);
                newLabel.GetComponent<Text>().text = Path.GetFileNameWithoutExtension(prefab.name);
                newLabel.transform.position = newPrefab.transform.position + Vector3.up * 0.1f; // Adjust position as needed
            }

            previousPosition = targetPosition;

            instantiatedPrefabs.Add(newPrefab);

            yield return new WaitForSeconds(SlideInDelay);
        }

        if (ResultText != null)
        {
            ResultText.text = "" + numberOfPrefabs;
        }

        if (FillButton != null)
        {
            FillButton.gameObject.SetActive(true);
        }

        // Stop the audio clip once all prefabs have been instantiated
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void OnFillButtonClicked()
    {
        StartCoroutine(FillWithLiquidPrefabs());
        FillButton.interactable = false; // Disable the button after it's been clicked
        FillButton.gameObject.SetActive(false); // Hide the button after it's been clicked
    }

    IEnumerator FillWithLiquidPrefabs()
    {
        for (int i = 0; i < instantiatedPrefabs.Count; i++)
        {
            yield return new WaitForSeconds(FillDelay);

            GameObject liquidPrefab = GetUnusedLiquidPrefab();
            GameObject newLiquid = Instantiate(liquidPrefab, ParentTransform);
            newLiquid.transform.localScale = PrefabScale;
            newLiquid.transform.localPosition = instantiatedPrefabs[i].transform.localPosition;
            newLiquid.transform.localPosition += Vector3.back * 0.01f;

            newLiquid.transform.localPosition = new Vector3(newLiquid.transform.localPosition.x, newLiquid.transform.localPosition.y, PrefabZPosition);

            usedLiquidPrefabs.Add(liquidPrefab);

            // Instantiate text label prefab for filled liquid prefabs
            if (System.Array.Exists(liquidPrefabs, element => element == liquidPrefab))
            {
                GameObject newLabel = Instantiate(textLabelPrefab, ParentTransform);
                newLabel.GetComponent<Text>().text = Path.GetFileNameWithoutExtension(liquidPrefab.name);
                newLabel.transform.position = newLiquid.transform.position + Vector3.up * 0.075f; // Adjust position as needed

            }
        }
    }

    GameObject GetUnusedLiquidPrefab()
    {
        ShuffleArray(liquidPrefabs);

        foreach (GameObject liquidPrefab in liquidPrefabs)
        {
            if (!usedLiquidPrefabs.Contains(liquidPrefab))
            {
                return liquidPrefab;
            }
        }

        return liquidPrefabs[UnityEngine.Random.Range(0, liquidPrefabs.Length)]; // Use UnityEngine.Random here
    }

    void ShuffleArray(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1); // Use UnityEngine.Random here
            GameObject temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }

    IEnumerator SlideInAnimation(GameObject prefab, Vector3 targetPosition)
    {
        float elapsedTime = 0;

        while (elapsedTime < SlideInDuration)
        {
            prefab.transform.localPosition = Vector3.Lerp(prefab.transform.localPosition, targetPosition, elapsedTime / SlideInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        prefab.transform.localPosition = targetPosition;
    }

    void OnBackButtonClicked()
    {
        StopAllCoroutines(); // Stop all coroutines to prevent unintended behavior
        ClearPrefabs(); // Clear instantiated prefabs and reset lists

        if (ResultText != null)
        {
            ResultText.text = "";
        }

        if (FillButton != null)
        {
            FillButton.gameObject.SetActive(false); // Ensure Fill button is hidden
            FillButton.interactable = true; // Enable the Fill button again
        }

        // Stop the audio clip if it's currently playing
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void ClearPrefabs()
    {
        foreach (Transform child in ParentTransform)
        {
            Destroy(child.gameObject);
        }

        instantiatedPrefabs.Clear();
        usedLiquidPrefabs.Clear();
    }
}
