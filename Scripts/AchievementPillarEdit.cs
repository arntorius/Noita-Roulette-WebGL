using UnityEngine;

public class AchievementPillarEdit : MonoBehaviour
{
    public GameObject container; // Assign your container GameObject in the inspector
    public int rows = 5; // Number of rows
    public int columns = 10; // Number of columns
    public float spacingX = 0.09f; // Spacing between sprites horizontally
    public float spacingY = 0.06f; // Spacing between sprites vertically
    public Vector3 scale = new Vector3(0.025f, 0.025f, 1); // Scale factor for sprites
    public Color hoverColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Color to apply when hovering over a sprite

    private GameObject[] spriteInstances; // Array to hold instantiated sprites
    private bool scriptStarted = false;

    private void Start()
    {
        StartDisplay(); // Start displaying sprites immediately
    }

    private void StartDisplay()
    {
        if (!scriptStarted)
        {
            // Load all prefabs from the folder
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Sprites/Achievement Cards/Prefabs");

            int totalSprites = rows * columns;
            int spritesCount = Mathf.Min(totalSprites, prefabs.Length);

            spriteInstances = new GameObject[spritesCount];

            // Calculate the size of each cell
            float cellWidth = (1f + spacingX) * scale.x; // Assuming each sprite has width 1 unit
            float cellHeight = (1f + spacingY) * scale.y; // Assuming each sprite has height 1 unit

            // Loop through each prefab and instantiate them in the container
            for (int i = 0; i < spritesCount; i++)
            {
                // Create a new instance of the prefab with a specified name
                GameObject instance = Instantiate(prefabs[i], container.transform);
                instance.name = prefabs[i].name; // Set the name of the instance to the original prefab name

                // Calculate row and column indices
                int row = i / columns;
                int column = i % columns;

                // Calculate position within the container
                float xPos = column * cellWidth;
                float yPos = -row * cellHeight;

                // Set position relative to container
                instance.transform.localPosition = new Vector3(xPos, yPos, 0.57f);

                // Set the scale of the sprite GameObject
                instance.transform.localScale = scale;

                spriteInstances[i] = instance;
            }

            scriptStarted = true;
        }
    }
}