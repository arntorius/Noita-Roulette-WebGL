using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] 
public class WordPair
{
    public string word;
    public string secondResult;
}

public class AlchemyRoulette : MonoBehaviour
{
    public Button rollButton;
    public Button stopButton;
    public Text resultText;
    public Text additionalResultText; 
    public RectTransform parentSpriteRect; 
    public int maxLines = 4; 
    private int originalFontSize; 



    public List<WordPair> wordPairs = new List<WordPair>(); 

    private int selectedIndex = -1;
    private bool isRolling = false;

    void Start()
    {
        rollButton.onClick.AddListener(StartRoll);
        stopButton.onClick.AddListener(StopRoll);

        if (parentSpriteRect == null)
        {
            Debug.LogError("Parent-Sprite-RectTransform nicht zugewiesen!");
        }

    }

    void StartRoll()
    {
        isRolling = true;
        StartCoroutine(RollAnimation());
    }

    void StopRoll()
    {
        isRolling = false;

        if (selectedIndex >= 0 && selectedIndex < wordPairs.Count)
        {
            resultText.text = wordPairs[selectedIndex].word;
            additionalResultText.text = wordPairs[selectedIndex].secondResult;
        }

        additionalResultText.fontSize = originalFontSize; 
        AdjustTextToFitSprite(additionalResultText, parentSpriteRect, maxLines);

    }
    void AdjustTextToFitSprite(Text textComponent, RectTransform spriteRect, int maxLines)
    {
       
        textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
      
        textComponent.resizeTextMaxSize = maxLines;

        float preferredHeight = spriteRect.rect.height;
        int currentFontSize = textComponent.fontSize;
        while (textComponent.preferredHeight > preferredHeight && currentFontSize > 0)
        {
            currentFontSize--;
            textComponent.fontSize = currentFontSize;
        }
    }
    IEnumerator RollAnimation()
    {
        while (isRolling)
        {
            selectedIndex = Random.Range(0, wordPairs.Count);
            resultText.text = wordPairs[selectedIndex].word;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
