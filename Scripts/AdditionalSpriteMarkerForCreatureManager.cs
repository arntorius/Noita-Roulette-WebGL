using UnityEngine;

public class AdditionalSpriteMarkerForCreatureManager : MonoBehaviour
{
    private GameObject spawnedSprite;
    private bool hasSpawned = false; // Flag to track if a sprite has already been spawned
    private bool hasSpawnedSprite = false;

    public GameObject newSpritePrefab; // Reference to the new sprite prefab to be instantiated

    // Method to set the newSpritePrefab variable
    public void SetNewSpritePrefab(GameObject spritePrefab)
    {
        newSpritePrefab = spritePrefab;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !hasSpawned) // Left mouse click and not already spawned
        {
            Debug.Log("Mouse Clicked on Additional Sprite Marker");
            SpawnNewSprite();
        }
        else if (Input.GetMouseButtonDown(1)) // Right mouse click
        {
            Debug.Log("Mouse Rightclicked on Additional Sprite Marker");
            DestroySpawnedSprite();
        }
    }

    public bool HasSpawnedSprite()
    {
        return hasSpawnedSprite;
    }

    public void SetHasSpawnedSprite(bool value)
    {
        hasSpawnedSprite = value;
    }

    private void SpawnNewSprite()
    {
        if (newSpritePrefab != null)
        {
            spawnedSprite = Instantiate(newSpritePrefab, transform.position, Quaternion.identity);
            // Set the parent of the new sprite object to the same parent as the additional sprite
            spawnedSprite.transform.SetParent(transform.parent);
            hasSpawned = true; // Set the flag to true after spawning
        }
        else
        {
            Debug.LogWarning("New sprite prefab not assigned in the inspector!");
        }
    }

    private void DestroySpawnedSprite()
    {
        if (spawnedSprite != null)
        {
            Debug.Log("Destroying Additional Sprite");
            Destroy(spawnedSprite);
            hasSpawned = false; // Reset the flag after destroying
        }
        else
        {
            Debug.LogWarning("No sprite spawned to destroy.");
        }
    }
}
