using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AlchemyExpert : MonoBehaviour
{
    [System.Serializable]
    public class WordAndSprite
    {
        public string word;
        public Sprite sprite;
    }

    // Inspector-Assignable Properties
    public Button rollButton;
    public Button stopButton;
    public Text wordDisplay1;
    public GameObject spriteContainer1;
    public Text wordDisplay2;
    public GameObject spriteContainer2;
    public List<WordAndSprite> wordAndSpriteList = new List<WordAndSprite>(); // Single list of word-sprite pairs
    public float stopDelay = 1.0f; // Delay between stopping the first and second rolling (in seconds)
    public float rollSpeed = 0.1f; // Speed of rolling (time between changes in seconds)
    public AudioSource audioSource; // Audio source to play the sound
    public AudioClip rollAudioClip; // Audio clip to play

    private bool isRolling1 = false;
    private bool isRolling2 = false;
    private Coroutine rollCoroutine1;
    private Coroutine rollCoroutine2;
    private SpriteRenderer spriteRenderer1;
    private SpriteRenderer spriteRenderer2;

    private int lastSelectedIndex1 = -1; // Variable to store the last selected index for roll 1

    void Start()
    {
        if (rollButton != null)
            rollButton.onClick.AddListener(StartRoll);
        if (stopButton != null)
            stopButton.onClick.AddListener(StopRoll);

        // Create SpriteRenderer components as children of the sprite containers
        if (spriteContainer1 != null)
        {
            spriteRenderer1 = spriteContainer1.AddComponent<SpriteRenderer>();
        }

        if (spriteContainer2 != null)
        {
            spriteRenderer2 = spriteContainer2.AddComponent<SpriteRenderer>();
        }
    }

    // Start Rolling Logic
    public void StartRoll()
    {
        if (isRolling1 || isRolling2)
        {
            // Stop any ongoing coroutines and reset states
            StopAllCoroutines();
            isRolling1 = false;
            isRolling2 = false;
        }

        if (wordAndSpriteList.Count > 0)
        {
            isRolling1 = true;
            isRolling2 = true;
            rollCoroutine1 = StartCoroutine(RollLoop(wordDisplay1, spriteRenderer1, 1));
            rollCoroutine2 = StartCoroutine(RollLoop(wordDisplay2, spriteRenderer2, 2));

            // Play the audio clip
            if (audioSource != null && rollAudioClip != null)
            {
                audioSource.Stop(); // Stop the current audio if playing
                audioSource.clip = rollAudioClip;
                audioSource.Play();
            }
        }
    }

    // Stop Rolling Logic
    public void StopRoll()
    {
        if (isRolling1 || isRolling2)
        {
            isRolling1 = false;
            if (rollCoroutine1 != null)
            {
                StopCoroutine(rollCoroutine1);
                rollCoroutine1 = null;
            }
            StartCoroutine(StopSecondRollWithDelay());
        }
    }

    // Coroutine to stop the second rolling with delay and stop the audio
    IEnumerator StopSecondRollWithDelay()
    {
        yield return new WaitForSeconds(stopDelay);

        isRolling2 = false;
        if (rollCoroutine2 != null)
        {
            StopCoroutine(rollCoroutine2);
            rollCoroutine2 = null;
        }

        // Stop the audio with a 3-second delay
        if (audioSource != null && audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0f);
            audioSource.Stop();
        }
    }

    // Roll Loop (Coroutine)
    IEnumerator RollLoop(Text wordDisplay, SpriteRenderer spriteRenderer, int rollIndex)
    {
        while ((rollIndex == 1 && isRolling1) || (rollIndex == 2 && isRolling2))
        {
            int randomIndex = GetRandomIndex(rollIndex);

            WordAndSprite chosenPair = wordAndSpriteList[randomIndex];

            wordDisplay.text = chosenPair.word;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = chosenPair.sprite;
            }

            if (rollIndex == 1)
            {
                lastSelectedIndex1 = randomIndex;
            }

            // Adjust roll speed (time between changes)
            yield return new WaitForSeconds(rollSpeed); // Use rollSpeed to control the speed of rolling
        }
    }

    // Get a random index, ensuring that roll 2 does not match roll 1
    int GetRandomIndex(int rollIndex)
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, wordAndSpriteList.Count);
        } while (rollIndex == 2 && randomIndex == lastSelectedIndex1);

        return randomIndex;
    }
}
