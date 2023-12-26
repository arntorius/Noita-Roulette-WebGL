using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeightedChallenge
{
    public string challenge;
    public float weight;
}

public class QuestSystem : MonoBehaviour
{
    [Header("Categories")]
    public WeightedChallenge[] bringGetChallenges;
    public WeightedChallenge[] toLocationChallenges;
    public WeightedChallenge[] completeAtChallenges;

    [Header("UI Elements")]
    public Text resultTextA;
    public Text resultTextB;
    public Text resultTextC;
    public Button rollButton;
    public Button backButton; // Assign the "Back" button in the Inspector

    [Header("Audio")]
    public AudioClip rollSound;
    public AudioClip resultSound;
    private AudioSource audioSource;
    private AudioSource resultAudioSource;

    public float rollSpeedMultiplier = 1.0f;
    public float resultSpeedMultiplier = 1.0f;
    [Range(0.0f, 1.0f)] public float resultVolume = 1.0f;

    private const int RollIterations = 60;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (rollSound != null && audioSource != null)
        {
            audioSource.clip = rollSound;
            audioSource.loop = true;
        }

        resultAudioSource = gameObject.AddComponent<AudioSource>();

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(RollChallenges);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(StopQuestSystem);
        }
    }

    private void StopQuestSystem()
    {
        // Stop all audio sources
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (resultAudioSource != null)
        {
            resultAudioSource.Stop();
        }

        // Stop the script
        StopAllCoroutines();

        // Optionally, you may want to reset the resultText components
        if (resultTextA != null)
        {
            resultTextA.text = string.Empty;
        }

        if (resultTextB != null)
        {
            resultTextB.text = string.Empty;
        }

        if (resultTextC != null)
        {
            resultTextC.text = string.Empty;
        }
    }

    private void RollChallenges()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            audioSource.pitch = 1.0f / rollSpeedMultiplier;
        }

        StartCoroutine(RollCategory(resultTextA, bringGetChallenges));
        StartCoroutine(RollCategory(resultTextB, toLocationChallenges));
        StartCoroutine(RollCategory(resultTextC, completeAtChallenges));
    }

    private IEnumerator RollCategory(Text resultText, WeightedChallenge[] challenges)
    {
        for (int i = 0; i < RollIterations; i++)
        {
            WeightedChallenge rolledChallenge = RollWeightedCategory(challenges);
            DisplayResult(resultText, rolledChallenge.challenge);
            yield return new WaitForSeconds(0.1f * rollSpeedMultiplier);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        WeightedChallenge finalChallenge = RollWeightedCategory(challenges);
        DisplayResult(resultText, finalChallenge.challenge);

        if (resultSound != null && resultAudioSource != null)
        {
            resultAudioSource.pitch = 1.0f / resultSpeedMultiplier;
            resultAudioSource.volume = resultVolume;
            resultAudioSource.PlayOneShot(resultSound);
        }
    }

    private WeightedChallenge RollWeightedCategory(WeightedChallenge[] challenges)
    {
        float totalWeight = 0f;
        foreach (var challenge in challenges)
        {
            totalWeight += challenge.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (var challenge in challenges)
        {
            if (randomValue < challenge.weight)
            {
                return challenge;
            }

            randomValue -= challenge.weight;
        }

        return challenges[Random.Range(0, challenges.Length)];
    }

    private void DisplayResult(Text resultText, string result)
    {
        if (resultText != null)
        {
            resultText.text = result;
        }
    }
}
