using UnityEngine;
using UnityEngine.UI;

public class TextScaler : MonoBehaviour
{
    [SerializeField]
    private RectTransform parentRectTransform; // Reference to the parent RectTransform

    private Text textField;

    private void Start()
    {
        textField = GetComponent<Text>();
        ScaleText();
    }

    private void Update()
    {
        ScaleText();
    }

    private void ScaleText()
    {
        if (parentRectTransform != null && textField != null)
        {
            // Calculate the scale factor based on the parent's size
            float parentWidth = parentRectTransform.rect.width;
            float parentHeight = parentRectTransform.rect.height;
            float textWidth = textField.preferredWidth;
            float textHeight = textField.preferredHeight;

            float widthScale = parentWidth / textWidth;
            float heightScale = parentHeight / textHeight;

            // Apply the minimum scale to ensure the text fits inside the bounds
            float minScale = Mathf.Min(widthScale, heightScale);
            minScale = Mathf.Clamp01(minScale); // Ensure it's between 0 and 1

            // Apply the scale to the text
            textField.rectTransform.localScale = new Vector3(minScale, minScale, 1f);
        }
    }
}
