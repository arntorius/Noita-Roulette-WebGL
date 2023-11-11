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

        // Register a callback for when the parent's size changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
        LayoutRebuilder.MarkLayoutForRebuild(parentRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
        parentRectTransform.GetComponent<LayoutGroup>().CalculateLayoutInputVertical();
        parentRectTransform.GetComponent<LayoutGroup>().SetLayoutHorizontal();
        parentRectTransform.GetComponent<LayoutGroup>().SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);

        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
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
