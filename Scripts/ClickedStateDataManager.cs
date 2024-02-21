using UnityEngine;

public class ClickedStateDataManager : MonoBehaviour
{
    public ClickedStateData clickedStateData; // Reference to the ClickedStateData scriptable object

    // Singleton instance
    private static ClickedStateDataManager instance;
    public static ClickedStateDataManager Instance => instance;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the object between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Method to get the ClickedStateData reference
    public ClickedStateData GetClickedStateData()
    {
        return clickedStateData;
    }
}
