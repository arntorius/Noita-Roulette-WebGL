using UnityEngine;
using UnityEngine.UI;

public class AchievementSpriteClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool clicked = false;
    public ClickedStateData clickedStateData; // Reference to the scriptable object

    private void Start()
    {
        if (clickedStateData == null)
        {
            clickedStateData = FindObjectOfType<ClickedStateData>();
        }

        if (clickedStateData == null)
        {
            Debug.LogError("ClickedStateData is not assigned or found in the scene!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        string spriteName = gameObject.name; // Use sprite name as the key
        clicked = clickedStateData.GetClickedState(spriteName); // Get clicked state from scriptable object
        UpdateSpriteColor(); // Update color based on clicked state
    }

    private void OnMouseDown()
    {
        clicked = !clicked;
        UpdateSpriteColor();
        SaveClickedState();
    }

    private void UpdateSpriteColor()
    {
        spriteRenderer.color = clicked ? Color.gray : originalColor;
    }

    private void SaveClickedState()
    {
        string spriteName = gameObject.name;
        clickedStateData.SetClickedState(spriteName, clicked); // Set clicked state in scriptable object
    }
}
