using UnityEngine;
using UnityEngine.UI;

public class StartupManager : MonoBehaviour
{
    public Text infoText;

    private const string APP_STARTED_KEY = "app_started";

    void Start()
    {
        // Check if the app has been started before
        if (!PlayerPrefs.HasKey(APP_STARTED_KEY))
        {
            // The app has not been started before, show the info text
            infoText.gameObject.SetActive(true);

            // Set the flag to indicate that the app has been started
            PlayerPrefs.SetInt(APP_STARTED_KEY, 1);
        }
        else
        {
            // The app has been started before, hide the info text
            infoText.gameObject.SetActive(false);
        }
    }
}
