using UnityEngine;
using UnityEngine.UI;

public class ChangePanelImage : MonoBehaviour
{
    public Button forwardButton; // Assign this in the Inspector for changing to the new image
    public Button backButton; // Assign this in the Inspector for reverting to the original image
    public Image panelImage; // Assign this in the Inspector (The Image component on your Panel)
    public Sprite newSprite; // Assign the new sprite in the Inspector

    // Variable to store the original sprite
    private Sprite originalSprite;

    void Start()
    {
        if (forwardButton != null)
        {
            forwardButton.onClick.AddListener(ChangeImage);
        }
        else
        {
            Debug.LogError("Forward Button is not assigned.");
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(RevertImage);
        }
        else
        {
            Debug.LogError("Back Button is not assigned.");
        }

        // Store the original sprite
        originalSprite = panelImage.sprite;
    }

    void ChangeImage()
    {
        if (panelImage != null)
        {
            if (newSprite != null)
            {
                panelImage.sprite = newSprite;
            }
            else
            {
                Debug.LogError("New sprite is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Panel image component is not assigned.");
        }

        // Ensure the panel remains visible
        panelImage.gameObject.SetActive(true);
    }

    void RevertImage()
    {
        if (panelImage != null)
        {
            // Set the image back to the original sprite
            panelImage.sprite = originalSprite;
        }
        else
        {
            Debug.LogError("Panel image component is not assigned.");
        }

        // Ensure the panel remains visible
        panelImage.gameObject.SetActive(true);
    }
}
