using UnityEngine;
using UnityEngine.UI;

public class OpenLinkButton : MonoBehaviour
{
    public string url; // URL to open

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenLink);
        }
        else
        {
            Debug.LogError("No Button component found on this GameObject.");
        }
    }

    void OpenLink()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogError("URL is not set.");
        }
    }
}
